////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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
using Proxxon.GCode.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxxon.GCode.Load
{
    public class LoadHPGL
    {
        CommandStream _stream;
        bool _IsPenUp;
		bool _lastIsPenUp;
		SpaceCoordinate _last=new SpaceCoordinate();
        int _color;

		SpaceCoordinate _minpt;
		SpaceCoordinate _maxpt;

		decimal _penDownZ = -0.5m;
		decimal _penUpZ = 1m;

        public LoadInfo LoadOptions  { get; set; }

		private void InitLoad()
		{
			_last = new SpaceCoordinate();
			_minpt = new SpaceCoordinate() { X = int.MaxValue, Y = int.MaxValue };
			_maxpt = new SpaceCoordinate() { X = int.MinValue, Y = int.MinValue };
			_stream = new CommandStream();
			_IsPenUp = true;
			_lastIsPenUp = false;
			_color = 0;
		}

		public void LoadHPLG(CommandList commands)
        {
			InitLoad();

			if (LoadOptions.AutoScale)
			{
				using (StreamReader sr = new StreamReader(LoadOptions.FileName))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						_stream.Line = line;
						if (!Command(commands,true))
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
			commands.Clear();

            using (StreamReader sr = new StreamReader(LoadOptions.FileName))
            {
				commands.AddCommand(new MxxCommand() { GCodeAdd = "m3" });
				commands.AddCommand(new SetParameterCommand() { GCodeAdd = "#1 = " + _penUpZ.ToString(CultureInfo.InvariantCulture) });
				commands.AddCommand(new SetParameterCommand() { GCodeAdd = "#2 = " + _penDownZ.ToString(CultureInfo.InvariantCulture) });
				
				string line;
                while ((line = sr.ReadLine()) != null)
                {
                    _stream.Line = line;
                    if (!Command(commands,false))
                    {
                        commands.Clear();
                        break;
                    }
                }

				if (!_lastIsPenUp)
				{
					Command r;
					r = new G00Command();
					r.AddVariableParam('Z', "1");
					commands.AddCommand(r);
				}

				commands.AddCommand(new MxxCommand() { GCodeAdd = "m5" } );
            }
			commands.UpdateCache();
        }

		private bool Command(CommandList commands, bool analyse)
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
                        SpaceCoordinate pt = GetSpaceCoordiante(cmdidx == 3);
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
								r = new G00Command();
								r.AddVariableParam('Z', "1");
							}
							else
							{
								r = new G01Command();
								r.AddVariableParam('Z', "2");
							}
							commands.AddCommand(r);
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
							commands.AddCommand(r);
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

        private SpaceCoordinate GetSpaceCoordiante(bool isRelativPoint)
        {
			SpaceCoordinate pt = new SpaceCoordinate();
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
		private void AdjustOrig(ref SpaceCoordinate pt)
		{
			if (LoadOptions.SwapXY)
			{
				decimal tmp = pt.X.Value;
				pt.X = pt.Y;
				pt.Y = -tmp;
			}
		}

        private void Adjust(ref SpaceCoordinate pt,bool isRelativPoint)
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
