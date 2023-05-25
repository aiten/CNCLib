﻿/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace CNCLib.GCode.Generate.Load;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using CNCLib.GCode.Generate.Commands;
using CNCLib.Logic.Abstraction.DTO;

using Framework.Drawing;
using Framework.Parser;

public partial class LoadHpgl : LoadBase
{
    readonly bool _DEBUG = false;

    #region Read PLT

    private IList<HpglCommand> ReadHpglCommandList()
    {
        var list = new List<HpglCommand>();
        using (StreamReader sr = GetStreamReader())
        {
            string line;
            var    last    = new Point3D();
            bool   isPenUp = true;

            while ((line = sr.ReadLine()) != null)
            {
                var parser = new Parser(line);

                var cmds = new[] { "PU", "PD", "PA", "PR" };
                while (!parser.IsEOF())
                {
                    parser.SkipSpaces();
                    int cmdIdx = parser.TryGetString(cmds);

                    if (cmdIdx >= 0)
                    {
                        switch (cmdIdx)
                        {
                            case 0:
                                isPenUp = true;
                                break;
                            case 1:
                                isPenUp = false;
                                break;
                        }

                        while (parser.IsNumber())
                        {
                            var pt = new Point3D();
                            pt.X = parser.GetInt() / 40.0;
                            parser.TryGetString(",");
                            pt.Y = parser.GetInt() / 40.0;

                            AdjustOrig(ref pt);

                            if (cmdIdx == 3) // move rel
                            {
                                pt.X += last.X;
                                pt.Y += last.Y;
                            }

                            list.Add(
                                new HpglCommand
                                {
                                    CommandType = isPenUp ? HpglCommand.HpglCommandType.PenUp : HpglCommand.HpglCommandType.PenDown,
                                    PointTo     = pt
                                });

                            last = pt;

                            parser.TryGetString(",");
                        }
                    }
                    else if (parser.SkipSpaces() == ';')
                    {
                        parser.Next();
                    }
                    else
                    {
                        var hpglCmd = parser.ReadString(new[] { ';' });
                        list.Add(new HpglCommand { CommandString = hpglCmd });
                    }
                }
            }
        }

        return list;
    }

    private void RemoveFirstPenUp(IList<HpglCommand> list)
    {
        // remove first PU0,0 PU50,50 PU 100,100 => autoScale problem
        var toRemoveList = list.TakeWhile(h => !h.IsPenCommand || !h.IsPenDownCommand).ToList();
        int countPenUp   = toRemoveList.Count(h => h.IsPenCommand);

        foreach (var h in toRemoveList)
        {
            if (h.IsPenCommand)
            {
                if (countPenUp > 1)
                {
                    list.Remove(h);
                }

                countPenUp--;
            }
        }
    }

    private void RemoveLastPenUp(IList<HpglCommand> list)
    {
        // remove last PU0,0 => autoScale problem
        var toRemoveList = list.Reverse().TakeWhile(h => !h.IsPenCommand || !h.IsPenDownCommand).ToList();

        foreach (var h in toRemoveList)
        {
            if (h.IsPenCommand)
            {
                list.Remove(h);
            }
        }
    }

    private void AdjustOrig(ref Point3D pt)
    {
        if (LoadOptions.SwapXY)
        {
            double tmp = pt.X0;
            pt.X = pt.Y;
            pt.Y = -tmp;
        }
    }

    #endregion

    #region Load

    bool _lastIsPenUp = false;
    bool _needSpeed   = false;

    public override void Load()
    {
        PreLoad();
        var list = ReadHpglCommandList();

        RemoveFirstPenUp(list);
        RemoveLastPenUp(list);
        Smooth.CalculateAngles(list, null);

        if (LoadOptions.AutoScale)
        {
            var autoScale = new AutoScale
            {
                LoadOptions = LoadOptions,
                LoadX       = this
            };

            autoScale.AutoScaleList(list);
        }

        if (LoadOptions.SmoothType != LoadOptions.SmoothTypeEnum.NoSmooth)
        {
            var smooth = new Smooth
            {
                LoadOptions = LoadOptions,
                LoadX       = this
            };

            list = smooth.SmoothList(list);
        }

        if (LoadOptions.ConvertType != LoadOptions.ConvertTypeEnum.NoConvert)
        {
            var invert = new InvertLine
            {
                LoadOptions = LoadOptions,
                LoadX       = this
            };

            list = invert.ConvertInvert(list);
        }

        AddComment("PenMoveType", LoadOptions.PenMoveType.ToString());

        switch (LoadOptions.PenMoveType)
        {
            case LoadOptions.PenType.CommandString:
                AddCommentForLaser();
                break;
            case LoadOptions.PenType.ZMove:
                AddComment("PenDownSpeed", LoadOptions.EngraveDownSpeed);
                AddComment("PenUpPos",     LoadOptions.EngravePosUp);
                AddComment("PenDownPos",   LoadOptions.EngravePosDown);
                break;
        }

        AddComment("Speed", LoadOptions.MoveSpeed.ToString());

        if (LoadOptions.PenMoveType == LoadOptions.PenType.ZMove)
        {
            AddCommands("M3");

            if (LoadOptions.EngravePosInParameter)
            {
                Commands.AddCommand(
                    new SetParameterCommand
                    {
                        ParameterNo = 1,
                        GCodeAdd    = LoadOptions.EngravePosUp.ToString(CultureInfo.InvariantCulture)
                    });
                Commands.AddCommand(
                    new SetParameterCommand
                    {
                        ParameterNo = 2,
                        GCodeAdd    = LoadOptions.EngravePosDown.ToString(CultureInfo.InvariantCulture)
                    });
            }
        }

        if (LoadOptions.MoveSpeed.HasValue)
        {
            var setSpeed = new G01Command();
            setSpeed.AddVariable('F', LoadOptions.MoveSpeed.Value);
            Commands.Add(setSpeed);
        }

        foreach (var cmd in list)
        {
            if (!Command(cmd))
            {
                Commands.Clear();
                break;
            }
        }

        if (!_lastIsPenUp)
        {
            LoadPenUp();
        }

        if (LoadOptions.PenMoveType == LoadOptions.PenType.ZMove)
        {
            AddCommands("M5");
        }

        PostLoad();
    }

    private bool Command(HpglCommand cmd)
    {
        bool isPenUp = true;

        if (cmd.IsPenCommand)
        {
            switch (cmd.CommandType)
            {
                case HpglCommand.HpglCommandType.PenDown:
                    isPenUp = false;
                    break;
                case HpglCommand.HpglCommandType.PenUp:
                    isPenUp = true;
                    break;
            }

            Point3D pt = Adjust(cmd.PointTo);

            if (isPenUp != _lastIsPenUp)
            {
                if (isPenUp)
                {
                    LoadPenUp();
                }
                else
                {
                    LoadPenDown(Adjust(cmd.PointFrom));
                }

                _lastIsPenUp = isPenUp;
            }

            string  hpglCmd;
            Command r;
            if (isPenUp)
            {
                r       = new G00Command();
                hpglCmd = "PU";
            }
            else
            {
                r = new G01Command();
                AddCamBamPoint(pt);
                hpglCmd = "PD";
            }

            r.AddVariable('X', pt.X0, false);
            r.AddVariable('Y', pt.Y0, false);
            if (_needSpeed)
            {
                _needSpeed = false;
                r.AddVariable('F', LoadOptions.MoveSpeed ?? 0);
            }

            Commands.AddCommand(r);

            r.ImportInfo = $"{hpglCmd}{(int)(pt.X0 * 40.0)},{(int)(pt.Y0 * 40.0)}";
        }
        else
        {
            var r = new GxxCommand();
            r.SetCode($";Hpgl={cmd.CommandString}");
            r.ImportInfo = cmd.CommandString;
            Commands.AddCommand(r);
        }

        return true;
    }

    private void LoadPenDown(Point3D pt)
    {
        if (LoadOptions.PenMoveType == LoadOptions.PenType.ZMove)
        {
            var r = new G01Command();
            if (LoadOptions.EngravePosInParameter)
            {
                r.AddVariableParam('Z', "2", false);
            }
            else
            {
                r.AddVariable('Z', LoadOptions.EngravePosDown);
            }

            if (LoadOptions.EngraveDownSpeed.HasValue)
            {
                r.AddVariable('F', LoadOptions.EngraveDownSpeed.Value);
                _needSpeed = LoadOptions.MoveSpeed.HasValue;
            }

            Commands.AddCommand(r);
        }
        else // if (LoadOptions.PenMoveType == LoadInfo.PenType.Command)
        {
            LaserOn();
        }

        AddCamBamPLine();
        AddCamBamPoint(pt);
    }

    private void LoadPenUp()
    {
        if (LoadOptions.PenMoveType == LoadOptions.PenType.ZMove)
        {
            var r = new G00Command();
            if (LoadOptions.EngravePosInParameter)
            {
                r.AddVariableParam('Z', "1", false);
            }
            else
            {
                r.AddVariable('Z', LoadOptions.EngravePosUp);
            }

            Commands.AddCommand(r);
        }
        else // if (LoadOptions.PenMoveType == LoadInfo.PenType.Command)
        {
            LaserOff();
        }

        FinishCamBamPLine();
    }

    private Point3D Adjust(Point3D pt)
    {
        var ret = new Point3D
        {
            X = pt.X,
            Y = pt.Y,
            Z = pt.Z
        };

        ret.X += (double)LoadOptions.OfsX;
        ret.Y += (double)LoadOptions.OfsY;

        if (LoadOptions.ScaleX != 0)
        {
            ret.X = Math.Round(ret.X0 * (double)LoadOptions.ScaleX, 3);
        }

        if (LoadOptions.ScaleY != 0)
        {
            ret.Y = Math.Round(ret.Y0 * (double)LoadOptions.ScaleY, 3);
        }

        return ret;
    }

    #endregion

    #region Debug-Helpers

    private void WriteLineToFile(IEnumerable<HpglCommand> list, int lineIdx)
    {
        if (list.Any())
        {
            var firstFrom = list.First().PointFrom;
            using (var sw = new StreamWriter(Environment.ExpandEnvironmentVariables($"%TEMP%\\CNCLib_Line{lineIdx}.plt")))
            {
                sw.WriteLine($"PU {(int)(firstFrom.X0 * 40)},{(int)(firstFrom.Y0 * 40)}");
                foreach (var cmd in list)
                {
                    sw.WriteLine($"PD {(int)(cmd.PointTo.X0 * 40)},{(int)(cmd.PointTo.Y0 * 40)}");
                }
            }
        }
    }

    #endregion
}