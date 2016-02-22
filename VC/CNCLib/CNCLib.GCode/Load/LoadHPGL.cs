////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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

using Framework.Tools;
using CNCLib.GCode.Commands;
using System;
using System.Globalization;
using System.IO;
using Framework.Tools.Drawing;

namespace CNCLib.GCode.Load
{
    public class LoadHPGL : LoadBase
    {
        CommandStream _stream;
        bool _IsPenUp;
		bool _lastIsPenUp;
		Point3D _last=new Point3D();
        int _color;
        bool _needSpeed;

        Point3D _minpt;
		Point3D _maxpt;

		private void InitLoad()
		{
			_last = new Point3D();
			_minpt = new Point3D() { X = int.MaxValue, Y = int.MaxValue };
			_maxpt = new Point3D() { X = int.MinValue, Y = int.MinValue };
			_stream = new CommandStream();
			_IsPenUp = true;
			_lastIsPenUp = false;
			_color = 0;
		}

		public override void Load()
        {
            InitLoad();

            AddFileHeader();

            if (LoadOptions.AutoScale)
			{
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
				LoadOptions.OfsX = -(_minpt.X.Value - LoadOptions.AutoScaleBorderDistX);
				LoadOptions.OfsY = -(_minpt.Y.Value - LoadOptions.AutoScaleBorderDistY);
				decimal sizex = _maxpt.X.Value - _minpt.X.Value + 2 * LoadOptions.AutoScaleBorderDistX;
				decimal sizey = _maxpt.Y.Value - _minpt.Y.Value + 2 * LoadOptions.AutoScaleBorderDistY;

				LoadOptions.ScaleX = LoadOptions.AutoScaleSizeX / sizex;
				LoadOptions.ScaleY = LoadOptions.AutoScaleSizeY / sizey;

				if (LoadOptions.AutoScaleKeepRatio)
				{
					LoadOptions.ScaleX =
					LoadOptions.ScaleY = Math.Min(LoadOptions.ScaleX, LoadOptions.ScaleY);
				}
			}

			InitLoad();
			Commands.Clear();

            AddFileHeader();
            Commands.Add(new GxxCommand() { GCodeAdd = "; File=" + LoadOptions.FileName });

            using (StreamReader sr = new StreamReader(LoadOptions.FileName))
            {
				if (LoadOptions.PenMoveType == LoadInfo.PenType.ZMove)
				{
                    AddCommands("M3");

					if (LoadOptions.PenPosInParameter)
					{
						Commands.AddCommand(new SetParameterCommand() { GCodeAdd = "#1 = " + LoadOptions.PenPosUp.ToString(CultureInfo.InvariantCulture) });
						Commands.AddCommand(new SetParameterCommand() { GCodeAdd = "#2 = " + LoadOptions.PenPosDown.ToString(CultureInfo.InvariantCulture) });
					}
				}

                if (LoadOptions.PenMoveSpeed.HasValue)
                {
                    var setspeed = new G01Command();
                    setspeed.AddVariable('F', LoadOptions.PenMoveSpeed.Value);
                    Commands.Add(setspeed);
                }
                _needSpeed = false;

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

				if (LoadOptions.PenMoveType == LoadInfo.PenType.ZMove)
				{
                    AddCommands("M5");
				}
            }
			Commands.UpdateCache();
        }

		private bool Command(bool analyse)
        {
            string[] cmds = new string[] { "PU", "PD", "PA", "PR", "SP" };
            while (!_stream.IsEOF())
            {
                int cmdidx = _stream.IsCommand(cmds);

                if (cmdidx==4)
                {
                    if (_stream.IsInt())
                    {
                        int coloridx = _stream.GetInt();
						if (coloridx >= 1 && coloridx <= 8)
							_color = (coloridx - 1); //  _pencolor[coloridx - 1];
                    }
                }
                else if (cmdidx >= 0)
                {
                    switch (cmdidx)
                    {
                        case 0: _IsPenUp = true; break;
                        case 1: _IsPenUp = false; break;
                    }

                    while (_stream.IsInt())
                    {
                        Point3D pt = GetSpaceCoordiante(cmdidx == 3);
                        if (cmdidx == 3)  // move rel
                        {
                            pt.X += _last.X;
                            pt.Y += _last.Y;
                        }

						if (!analyse && _IsPenUp != _lastIsPenUp)
						{
							Command r;
							if (_IsPenUp)
                            {
                                LoadPenUp();
                            }
                            else
                            {
                                LoadPenDown();
                            }
							_lastIsPenUp = _IsPenUp;
						}

						_last = pt;

						if (!analyse)
						{
							Command r;
							if (_IsPenUp)
							{
								r = new G00Command();
							}
							else
							{
								r = new G01Command();
							}
							r.AddVariable('X', pt.X.Value);
							r.AddVariable('Y', pt.Y.Value);
                            if (_needSpeed)
                            {
                                _needSpeed = false;
                                r.AddVariable('F', LoadOptions.PenMoveSpeed.Value);
                            }
                            Commands.AddCommand(r);
						}

                        _stream.IsCommand(",");
                    }
                }
                else
                {
                    // skip command
                    _stream.SkipEndCommand();
                }
            }

            return true;
        }

        private void LoadPenDown()
        {
            if (LoadOptions.PenMoveType == LoadInfo.PenType.ZMove)
            {
                var r = new G01Command();
                if (LoadOptions.PenPosInParameter)
                {
                    r.AddVariableParam('Z', "2");
                }
                else
                {
                    r.AddVariable('Z', LoadOptions.PenPosDown);
                }
                if (LoadOptions.PenDownSpeed.HasValue)
                {
                    r.AddVariable('F', LoadOptions.PenDownSpeed.Value);
                    _needSpeed = LoadOptions.PenMoveSpeed.HasValue;
                }
                Commands.AddCommand(r);
            }
            else // if (LoadOptions.PenMoveType == LoadInfo.PenType.Command)
            {
                AddCommands(LoadOptions.PenDownCommandString);
            }
        }

        private void LoadPenUp()
        {
            if (LoadOptions.PenMoveType == LoadInfo.PenType.ZMove)
            {
                var r = new G00Command();
                if (LoadOptions.PenPosInParameter)
                {
                    r.AddVariableParam('Z', "1");
                }
                else
                {
                    r.AddVariable('Z', LoadOptions.PenPosUp);
                }
                Commands.AddCommand(r);
            }
            else // if (LoadOptions.PenMoveType == LoadInfo.PenType.Command)
            {
                AddCommands(LoadOptions.PenUpCommandString);
            }
        }

        private Point3D GetSpaceCoordiante(bool isRelativPoint)
        {
			Point3D pt = new Point3D();
            pt.X = _stream.GetInt() / 40m;
			_stream.IsCommand(",") ;
			pt.Y = _stream.GetInt() / 40m;

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
				decimal tmp = pt.X.Value;
				pt.X = pt.Y;
				pt.Y = -tmp;
			}
		}

        private void Adjust(ref Point3D pt,bool isRelativPoint)
        {
            if (!isRelativPoint)
            {
                pt.X += LoadOptions.OfsX;
                pt.Y += LoadOptions.OfsY;
            }

            if (LoadOptions.ScaleX != 0)
				pt.X = Math.Round(pt.X.Value * LoadOptions.ScaleX, 3);
            if (LoadOptions.ScaleY != 0)
				pt.Y = Math.Round(pt.Y.Value * LoadOptions.ScaleY, 3);
        }

    }
}
