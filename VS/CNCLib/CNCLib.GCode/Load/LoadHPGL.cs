////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
using Framework.Tools;
using Framework.Tools.Drawing;

namespace CNCLib.GCode.Load
{
	public class LoadHPGL : LoadBase
    {
        bool _DEBUG = false;

        #region HPGLCommand

        class HPGLCommand
        {
            public enum HPGLCommandType
            {
                PenUp,
                PenDown,
                Other
            };
            public HPGLCommandType CommandType { get; set; } = HPGLCommandType.Other;
            public bool IsPenCommand { get { return CommandType == HPGLCommandType.PenUp || CommandType == HPGLCommandType.PenDown; } }
            public bool IsPenDownCommand { get { return CommandType == HPGLCommandType.PenDown; } }
            public bool IsPointToValid { get { return IsPenCommand; } }
            public Point3D PointFrom { get; set; }
            public Point3D PointTo { get; set; }
            public double? LineAngle { get; set; }
            public double? DiffLineAngleWithNext { get; set; }
            public string CommandString { get; set; }

            public void ResetCalculated()
            {
                PointFrom = null;
                DiffLineAngleWithNext = null;
                LineAngle = null;
            }
        }

        #endregion

        #region Read PLT

        private IList<HPGLCommand> ReadHPGLCommandList()
        {
            var list = new List<HPGLCommand>();
            using (StreamReader sr = new StreamReader(LoadOptions.FileName))
            {
                string line;
                Point3D last = new Point3D();
                bool isPenUp = true;
                CommandStream stream = new CommandStream();

                while ((line = sr.ReadLine()) != null)
                {
                    stream.Line = line;

                    string[] cmds = new string[] { "PU", "PD", "PA", "PR" };
                    while (!stream.IsEOF())
                    {
                        stream.SkipSpaces();
                        int idx = stream.PushIdx();
                        int cmdidx = stream.IsCommand(cmds);

                        if (cmdidx >= 0)
                        {
                            switch (cmdidx)
                            {
                                case 0: isPenUp = true; break;
                                case 1: isPenUp = false; break;
                            }

                            while (stream.IsNumber())
                            {
                                 Point3D pt = new Point3D();
                                pt.X = stream.GetInt() / 40.0;
                                stream.IsCommand(",");
                                pt.Y = stream.GetInt() / 40.0;

                                AdjustOrig(ref pt);
 
                                if (cmdidx == 3)  // move rel
                                {
                                    pt.X += last.X;
                                    pt.Y += last.Y;
                                }

                                list.Add(new HPGLCommand()
                                {
                                    CommandType = isPenUp ? HPGLCommand.HPGLCommandType.PenUp : HPGLCommand.HPGLCommandType.PenDown,
                                    PointTo = pt
                                }
                                );

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
                            var hpglcmd = stream.ReadString(new char[] { ';' });
                            list.Add(new HPGLCommand() { CommandString = hpglcmd });
                        }
                    }
                }
            }

            CalculateAngles(list,null);

            return list;
        }

        private void CalculateAngles(IEnumerable<HPGLCommand> list, Point3D firstfrom)
        {
            HPGLCommand last = null;
            if (firstfrom != null)
                last = new HPGLCommand()
                {
                    PointTo = firstfrom,
                    CommandType = HPGLCommand.HPGLCommandType.PenDown
                };

            foreach(var cmd in list)
            {
                cmd.ResetCalculated();
                if (cmd.IsPointToValid)
                {
                    if (last != null)
                    {
                        cmd.PointFrom = last.PointTo;
                        cmd.LineAngle = Math.Atan2((cmd.PointTo.Y ?? 0.0) - (cmd.PointFrom.Y ?? 0.0), (cmd.PointTo.X ?? 0.0) - (cmd.PointFrom.X ?? 0.0));
                        cmd.DiffLineAngleWithNext = null;

                        if (last.LineAngle.HasValue && cmd.IsPenDownCommand)
                        {
                            last.DiffLineAngleWithNext = last.LineAngle - cmd.LineAngle;
                            if (last.DiffLineAngleWithNext > Math.PI)
                                last.DiffLineAngleWithNext -= Math.PI * 2.0;

                            if (last.DiffLineAngleWithNext < -Math.PI)
                                last.DiffLineAngleWithNext += (Math.PI*2.0);
                        }
                    }
                    last = cmd;
                }
            }
        }

        private void AdjustOrig(ref Point3D pt)
        {
            if (LoadOptions.SwapXY)
            {
                var tmp = pt.X.Value;
                pt.X = pt.Y;
                pt.Y = -tmp;
            }
        }

        #endregion

        #region Load

        bool _lastIsPenUp = false;
        bool _needSpeed = false;

        public override void Load()
        {
            PreLoad();
            var list = ReadHPGLCommandList();

            if (LoadOptions.AutoScale)
			{
				AutoScale(list);
			}

            if (LoadOptions.SmoothType != LoadOptions.SmoothTypeEnum.NoSmooth)
            {
                list = Smooth(list);
            }

            AddComment("PenMoveType" , LoadOptions.PenMoveType.ToString() );

            switch (LoadOptions.PenMoveType)
            {
                case LoadOptions.PenType.CommandString:
					AddCommentForLaser();
                    break;
                case LoadOptions.PenType.ZMove:
                    AddComment("PenDownSpeed" , LoadOptions.EngraveDownSpeed );
                    AddComment("PenUpPos" ,     LoadOptions.EngravePosUp );
                    AddComment("PenDownPos" ,   LoadOptions.EngravePosDown );
                    break;
            }

            AddComment("Speed" , LoadOptions.MoveSpeed.ToString() );

			if (LoadOptions.PenMoveType == LoadOptions.PenType.ZMove)
			{
                AddCommands("M3");

				if (LoadOptions.EngravePosInParameter)
				{
					Commands.AddCommand(new SetParameterCommand() { GCodeAdd = "#1 = " + LoadOptions.EngravePosUp.ToString(CultureInfo.InvariantCulture) } );
					Commands.AddCommand(new SetParameterCommand() { GCodeAdd = "#2 = " + LoadOptions.EngravePosDown.ToString(CultureInfo.InvariantCulture) });
				}
			}

            if (LoadOptions.MoveSpeed.HasValue)
            {
                var setspeed = new G01Command();
                setspeed.AddVariable('F', LoadOptions.MoveSpeed.Value);
                Commands.Add(setspeed);
            }

            foreach(var cmd in list)
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
                    case HPGLCommand.HPGLCommandType.PenDown: isPenUp = false; break;
                    case HPGLCommand.HPGLCommandType.PenUp: isPenUp = true; break;
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

                string hpglCmd;
                Command r;
                if (isPenUp)
                {
                    r = new G00Command();
                    hpglCmd = "PU";
                }
                else
                {
                    r = new G01Command();
                    AddCamBamPoint(pt);
                    hpglCmd = "PD";
                }
                r.AddVariable('X', pt.X.Value, false);
                r.AddVariable('Y', pt.Y.Value, false);
                if (_needSpeed)
                {
                    _needSpeed = false;
                    r.AddVariable('F', LoadOptions.MoveSpeed.Value);
                }
                Commands.AddCommand(r);

                r.ImportInfo = $"{hpglCmd}{(int)(pt.X.Value * 40.0)},{(int)(pt.Y.Value * 40.0)}";
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
                    r.AddVariableParam('Z', "2");
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
                    r.AddVariableParam('Z', "1");
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
                ret.X = Math.Round(ret.X.Value * (double)LoadOptions.ScaleX, 3);
            if (LoadOptions.ScaleY != 0)
                ret.Y = Math.Round(ret.Y.Value * (double)LoadOptions.ScaleY, 3);

            return ret;
        }

        #endregion

        #region AutoScale

        private void AutoScale(IList<HPGLCommand> list)
		{
			AddComment("AutoScaleX", LoadOptions.AutoScaleSizeX);
			AddComment("AutoScaleY", LoadOptions.AutoScaleSizeY);

			AddComment("AutoScaleDistX", LoadOptions.AutoScaleBorderDistX);
			AddComment("AutoScaleDistY", LoadOptions.AutoScaleBorderDistY);

            AddComment("AutoScaleCenter", LoadOptions.AutoScaleCenter.ToString());

            var minpt = new Point3D()
            {
                X = list.Where((x) => x.IsPenCommand).Min((c) => c.PointTo.X),
                Y = list.Where((x) => x.IsPenCommand).Min((c) => c.PointTo.Y)
            };
            var maxpt = new Point3D()
            {
                X = list.Where((x) => x.IsPenCommand).Max((c) => c.PointTo.X),
                Y = list.Where((x) => x.IsPenCommand).Max((c) => c.PointTo.Y)
            };

            decimal sizex = (decimal)maxpt.X.Value - (decimal)minpt.X.Value;
			decimal sizey = (decimal)maxpt.Y.Value - (decimal)minpt.Y.Value;

            decimal borderX = LoadOptions.AutoScaleBorderDistX;
            decimal borderY = LoadOptions.AutoScaleBorderDistY;

            decimal destSizeX = LoadOptions.AutoScaleSizeX - 2m * borderX;
            decimal destSizeY = LoadOptions.AutoScaleSizeY - 2m * borderY;

            LoadOptions.ScaleX = destSizeX / sizex;
			LoadOptions.ScaleY = destSizeY / sizey;

            if (LoadOptions.AutoScaleKeepRatio)
			{
				LoadOptions.ScaleX =
				LoadOptions.ScaleY = Math.Min(LoadOptions.ScaleX, LoadOptions.ScaleY);

                if (LoadOptions.AutoScaleCenter)
                {
                    decimal sizeXscaled = LoadOptions.ScaleX * sizex;
                    decimal sizeYscaled = LoadOptions.ScaleY * sizey;

                    borderX += (destSizeX - sizeXscaled) / 2m;
                    borderY += (destSizeY - sizeYscaled) / 2m;
                }
            }

            LoadOptions.OfsX = -((decimal)minpt.X.Value - borderX / LoadOptions.ScaleX);
            LoadOptions.OfsY = -((decimal)minpt.Y.Value - borderY / LoadOptions.ScaleY);
        }

        #endregion

        #region Smooth 

        private IList<HPGLCommand> Smooth(IList<HPGLCommand> list)
        {
            var newlist = new List<HPGLCommand>();

            int startidx = 0;
            while (startidx < list.Count())
            {
                var nopenlist = list.Skip(startidx).TakeWhile((e) => !e.IsPenDownCommand);
                newlist.AddRange(nopenlist);
                startidx += nopenlist.Count();

                var line = list.Skip(startidx).TakeWhile((e) => e.IsPenDownCommand);
                startidx += line.Count();

                newlist.AddRange(SmoothLine(line));
            }

            CalculateAngles(newlist,null);
            return newlist;
        }

        int lineidx = 1;
        private IList<HPGLCommand> SmoothLine(IEnumerable<HPGLCommand> line)
        {
            if (_DEBUG)
                WriteLineToFile(line, lineidx++);

            var list = new List<HPGLCommand>();
            double maxAngle = LoadOptions.SmoothMinAngle.HasValue ? (double)LoadOptions.SmoothMinAngle.Value : (45 * (Math.PI / 180));

            int startidx = 0;
            while (startidx < line.Count())
            {
                // check for angle
                var linepart = line.Skip(startidx).TakeWhile((c) => Math.Abs(c.DiffLineAngleWithNext??(0.0)) < maxAngle);
                if (linepart.Count() > 0)
                {
                    startidx += linepart.Count();
                    list.AddRange(SplitLine(linepart));
                }
                else
                {
                    linepart = line.Skip(startidx).TakeWhile((c) => Math.Abs(c.DiffLineAngleWithNext??(0.0)) >= maxAngle);
                    startidx += linepart.Count();
                    list.AddRange(linepart);
                }
            }
            return list;
        }
        private IEnumerable<HPGLCommand> SplitLine(IEnumerable<HPGLCommand> line)
        {
            if (line.Count() < 2)
                return line;

            Point3D firstfrom = line.ElementAt(0).PointFrom;
            for (int i = 0; i < 100; i++)
            {
                var newline = SplitLineImpl(line);
                if (newline.Count() == line.Count())
                    return newline;
                line = newline;
                CalculateAngles(line, firstfrom);
            }
            return line;
        }

        private IEnumerable<HPGLCommand> SplitLineImpl(IEnumerable<HPGLCommand> line)
        {
            if (line.Count() < 3)
                return line;

            var newline = new List<HPGLCommand>();
            HPGLCommand prev=null;
            double minLineLenght = LoadOptions.SmoothMinLineLenght.HasValue ? (double) LoadOptions.SmoothMinLineLenght.Value : double.MaxValue;
            double maxerror = LoadOptions.SmoothMaxError.HasValue ? (double)LoadOptions.SmoothMaxError.Value : 1.0/40.0;
            minLineLenght /= (double) LoadOptions.ScaleX;
            maxerror /= (double)LoadOptions.ScaleX;

            foreach (var pt in line)
            {
                double x = (pt.PointTo.X??0.0) - (pt.PointFrom.X??0.0);
                double y = (pt.PointTo.Y??0.0) - (pt.PointFrom.Y??0.0);

                var c = Math.Sqrt(x * x + y * y);

                if (minLineLenght <= c)
                {
                    double alpha = pt.DiffLineAngleWithNext ?? (prev?.DiffLineAngleWithNext ?? 0.0);
                    double beta = prev != null ? (prev.DiffLineAngleWithNext ?? 0.0) : alpha;
                    double swapscale = 1.0;

                    if ((alpha >= 0.0 && beta >= 0.0) || (alpha <= 0.0 && beta <= 0.0))
                    {

                    }
                    else
                    {
                        beta = -beta;
                        swapscale = 0.5;
                    }
                    if ((alpha >= 0.0 && beta >= 0.0) || (alpha <= 0.0 && beta <= 0.0))
                    {
                        double gamma = Math.PI - alpha - beta;

                        double b = Math.Sin(beta) / Math.Sin(gamma) * c;
                        //double a = Math.Sin(alpha) / Math.Sin(gamma) * c;

                        double hc = b * Math.Sin(alpha) * swapscale;
                        double dc = Math.Sqrt(b * b - hc * hc);
                        double hc4 = hc / 4.0;

                        if (Math.Abs(hc4) > maxerror && Math.Abs(hc4) < c && Math.Abs(dc)< c )
                        {
                            newline.Add(GetNewCommand(pt, dc, hc4));
                        }
                    }
                }
                prev = pt;
                newline.Add(pt);
            }

            return newline;
        }

        /// <summary>
        /// Create a new point based on a existing line (x,y are based on the line vector)
        /// </summary>
        private HPGLCommand GetNewCommand(HPGLCommand pt, double x, double y)
        {
            double diffalpha = Math.Atan2(y, x);
            double linealpha = diffalpha + (pt.LineAngle ?? 0);

            double dx = x * Math.Cos(linealpha);
            double dy = x * Math.Sin(linealpha);

            return new HPGLCommand()
            {
                CommandType = pt.CommandType,
                PointTo = new Point3D() { X = pt.PointFrom.X + dx, Y = pt.PointFrom.Y + dy }
            };
        }

        #endregion

        #region Debug-Helpers

        private void WriteLineToFile(IEnumerable<HPGLCommand> list, int lineIdx)
        {
            if (list.Count() > 0)
            {
                var firstfrom = list.First().PointFrom;
                using (StreamWriter sw = new StreamWriter(Environment.ExpandEnvironmentVariables($"%TMP%\\CNCLib_Line{lineIdx}.plt")))
                {
                    sw.WriteLine($"PU {(int) (firstfrom.X.Value*40)},{(int) (firstfrom.Y.Value*40)}");
                    foreach (var cmd in list)
                    {
                        sw.WriteLine($"PD {(int) (cmd.PointTo.X.Value*40)},{(int) (cmd.PointTo.Y.Value*40)}");
                    }
                }
            }
        }

        #endregion

    }
}
