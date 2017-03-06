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
using System.Globalization;
using System.IO;
using CNCLib.GCode.Commands;
using CNCLib.Logic.Contracts.DTO;
using Framework.Tools;
using Framework.Tools.Drawing;

namespace CNCLib.GCode.Load
{
	public class LoadHPGL : LoadBase
    {
        CommandStream _stream = new CommandStream();
        bool _IsPenUp = true;
		bool _lastIsPenUp = false;
        int _color = 0;
        bool _needSpeed = false;

        Point3D _last = new Point3D();
        Point3D _minpt = new Point3D() { X = int.MaxValue, Y = int.MaxValue };
        Point3D _maxpt = new Point3D() { X = int.MinValue, Y = int.MinValue };

        public override void Load()
        {
            PreLoad();

            if (LoadOptions.AutoScale)
			{
				AutoScale();
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

            using (StreamReader sr = new StreamReader(LoadOptions.FileName))
            {
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

				string line;
                while ((line = sr.ReadLine()) != null)
                {
                    _stream.Line = line;
                    if (!Command(false))
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
            }
			PostLoad();
        }

		private void AutoScale()
		{
			AddComment("AutoScaleX", LoadOptions.AutoScaleSizeX);
			AddComment("AutoScaleY", LoadOptions.AutoScaleSizeY);

			AddComment("AutoScaleDistX", LoadOptions.AutoScaleBorderDistX);
			AddComment("AutoScaleDistY", LoadOptions.AutoScaleBorderDistY);

			using (StreamReader sr = new StreamReader(LoadOptions.FileName))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					_stream.Line = line;
					if (!Command(true))
					{
						break;
					}
				}
			}
			LoadOptions.OfsX = -((decimal) _minpt.X.Value - LoadOptions.AutoScaleBorderDistX);
			LoadOptions.OfsY = -((decimal)_minpt.Y.Value - LoadOptions.AutoScaleBorderDistY);
			decimal sizex = (decimal)_maxpt.X.Value - (decimal)_minpt.X.Value + 2 * LoadOptions.AutoScaleBorderDistX;
			decimal sizey = (decimal)_maxpt.Y.Value - (decimal)_minpt.Y.Value + 2 * LoadOptions.AutoScaleBorderDistY;

			LoadOptions.ScaleX = LoadOptions.AutoScaleSizeX / sizex;
			LoadOptions.ScaleY = LoadOptions.AutoScaleSizeY / sizey;

			if (LoadOptions.AutoScaleKeepRatio)
			{
				LoadOptions.ScaleX =
				LoadOptions.ScaleY = Math.Min(LoadOptions.ScaleX, LoadOptions.ScaleY);
			}
		}

		private bool Command(bool analyse)
        {
            string[] cmds = new string[] { "PU", "PD", "PA", "PR", "SP" };
            while (!_stream.IsEOF())
            {
				_stream.SkipSpaces();
				int idx = _stream.PushIdx();
				int cmdidx = _stream.IsCommand(cmds);

                if (cmdidx==4)
                {
                    if (_stream.IsNumber())
                    {
                        int coloridx = _stream.GetInt();
						if (coloridx >= 1 && coloridx <= 8)
							_color = (coloridx - 1); //  _pencolor[coloridx - 1];
                    }
					if (!analyse)
					{
						_stream.PopIdx(idx);
						HpglCommandToEnd(analyse);
					}
				}
				else if (cmdidx >= 0)
                {
                    switch (cmdidx)
                    {
                        case 0: _IsPenUp = true; break;
                        case 1: _IsPenUp = false; break;
                    }

                    while (_stream.IsNumber())
                    {
                        Point3D pt = GetSpaceCoordiante(cmdidx == 3);
                        if (cmdidx == 3)  // move rel
                        {
                            pt.X += _last.X;
                            pt.Y += _last.Y;
                        }

						if (!analyse && _IsPenUp != _lastIsPenUp)
						{
							if (_IsPenUp)
                            {
                                LoadPenUp();
                            }
                            else
                            {
                                LoadPenDown(_last);
                            }
							_lastIsPenUp = _IsPenUp;
						}

						_last = pt;

						if (!analyse)
						{
							string hpglCmd;
							Command r;
							if (_IsPenUp)
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

							r.ImportInfo = $"{hpglCmd}{(int) (pt.X.Value*40.0)},{(int) (pt.Y.Value*40.0)}";
						}

                        _stream.IsCommand(",");
                    }
                }
                else if (_stream.SkipSpaces()==';')
				{
					_stream.Next();
				}
				else
				{
					HpglCommandToEnd(analyse);
				}
			}

            return true;
        }

		private void HpglCommandToEnd(bool analyse)
		{
			var hpglcmd = _stream.ReadString(new char[] { ';' });
			if (!analyse)
				NewHpglCommand(hpglcmd);
		}

		private void NewHpglCommand(string hpglcmd)
		{
			var r = new GxxCommand();
			r.SetCode($";HPGL={hpglcmd}");
			r.ImportInfo = hpglcmd;
			Commands.AddCommand(r);
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

        private Point3D GetSpaceCoordiante(bool isRelativPoint)
        {
			Point3D pt = new Point3D();
            pt.X = _stream.GetInt() / 40.0;
			_stream.IsCommand(",") ;
			pt.Y = _stream.GetInt() / 40.0;

			AdjustOrig(ref pt);

			if (pt.X != 0 || pt.Y != 0)
			{
				if (_minpt.X > pt.X) _minpt.X = pt.X;
				if (_minpt.Y > pt.Y) _minpt.Y = pt.Y;

				if (_maxpt.X < pt.X) _maxpt.X = pt.X;
				if (_maxpt.Y < pt.Y) _maxpt.Y = pt.Y;
			}

            Adjust(ref pt, isRelativPoint);
            return pt;
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

        private void Adjust(ref Point3D pt,bool isRelativPoint)
        {
            if (!isRelativPoint)
            {
                pt.X += (double) LoadOptions.OfsX;
                pt.Y += (double) LoadOptions.OfsY;
            }

            if (LoadOptions.ScaleX != 0)
				pt.X = Math.Round(pt.X.Value * (double)LoadOptions.ScaleX, 3);
            if (LoadOptions.ScaleY != 0)
				pt.Y = Math.Round(pt.Y.Value * (double)LoadOptions.ScaleY, 3);
        }
    }
}
