////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using CNCLib.GCode.Commands;
using CNCLib.Logic.Contracts.DTO;

using Framework.Drawing;
using Framework.Parser;

namespace CNCLib.GCode.Load
{
    public partial class LoadHPGL : LoadBase
    {
        readonly bool _DEBUG = false;

        #region Read PLT

        private IList<HPGLCommand> ReadHPGLCommandList()
        {
            var list = new List<HPGLCommand>();
            using (StreamReader sr = GetStreamReader())
            {
                string line;
                var    last    = new Point3D();
                bool   isPenUp = true;
                var    stream  = new CommandStream();

                while ((line = sr.ReadLine()) != null)
                {
                    stream.Line = line;

                    var cmds = new[] { "PU", "PD", "PA", "PR" };
                    while (!stream.IsEOF())
                    {
                        stream.SkipSpaces();
                        int cmdIdx = stream.IsCommand(cmds);

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

                            while (stream.IsNumber())
                            {
                                var pt = new Point3D();
                                pt.X = stream.GetInt() / 40.0;
                                stream.IsCommand(",");
                                pt.Y = stream.GetInt() / 40.0;

                                AdjustOrig(ref pt);

                                if (cmdIdx == 3) // move rel
                                {
                                    pt.X += last.X;
                                    pt.Y += last.Y;
                                }

                                list.Add(new HPGLCommand
                                {
                                    CommandType = isPenUp ? HPGLCommand.HPGLCommandType.PenUp : HPGLCommand.HPGLCommandType.PenDown,
                                    PointTo     = pt
                                });

                                last = pt;

                                stream.IsCommand(",");
                            }
                        }
                        else if (stream.SkipSpaces() == ';')
                        {
                            stream.Next();
                        }
                        else
                        {
                            var hpglCmd = stream.ReadString(new[] { ';' });
                            list.Add(new HPGLCommand { CommandString = hpglCmd });
                        }
                    }
                }
            }

            return list;
        }

        private void RemoveFirstPenUp(IList<HPGLCommand> list)
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

        private void RemoveLastPenUp(IList<HPGLCommand> list)
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
            var list = ReadHPGLCommandList();

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
                    Commands.AddCommand(new SetParameterCommand
                    {
                        ParameterNo = 1,
                        GCodeAdd    = LoadOptions.EngravePosUp.ToString(CultureInfo.InvariantCulture)
                    });
                    Commands.AddCommand(new SetParameterCommand
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

        private bool Command(HPGLCommand cmd)
        {
            bool isPenUp = true;

            if (cmd.IsPenCommand)
            {
                switch (cmd.CommandType)
                {
                    case HPGLCommand.HPGLCommandType.PenDown:
                        isPenUp = false;
                        break;
                    case HPGLCommand.HPGLCommandType.PenUp:
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

                r.ImportInfo = $"{hpglCmd}{(int) (pt.X0 * 40.0)},{(int) (pt.Y0 * 40.0)}";
            }
            else
            {
                var r = new GxxCommand();
                r.SetCode($";HPGL={cmd.CommandString}");
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

            ret.X += (double) LoadOptions.OfsX;
            ret.Y += (double) LoadOptions.OfsY;

            if (LoadOptions.ScaleX != 0)
            {
                ret.X = Math.Round(ret.X0 * (double) LoadOptions.ScaleX, 3);
            }

            if (LoadOptions.ScaleY != 0)
            {
                ret.Y = Math.Round(ret.Y0 * (double) LoadOptions.ScaleY, 3);
            }

            return ret;
        }

        #endregion

        #region Debug-Helpers

        private void WriteLineToFile(IEnumerable<HPGLCommand> list, int lineIdx)
        {
            if (list.Any())
            {
                var firstFrom = list.First().PointFrom;
                using (var sw = new StreamWriter(Environment.ExpandEnvironmentVariables($"%TMP%\\CNCLib_Line{lineIdx}.plt")))
                {
                    sw.WriteLine($"PU {(int) (firstFrom.X0 * 40)},{(int) (firstFrom.Y0 * 40)}");
                    foreach (var cmd in list)
                    {
                        sw.WriteLine($"PD {(int) (cmd.PointTo.X0 * 40)},{(int) (cmd.PointTo.Y0 * 40)}");
                    }
                }
            }
        }

        #endregion
    }
}