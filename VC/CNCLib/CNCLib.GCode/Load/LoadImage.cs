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
using System.IO;
using System;

namespace CNCLib.GCode.Load
{
    public class LoadImage : LoadBase
    {
        CommandList _commands;
        bool _laserOn = true;
        double _pixelSizeX = 1;
        double _pixelSizeY = 1;
        int _sizeX;
        int _sizeY;
        double _shiftX = 0;
        double _shiftY = 0;
        double _shiftLaserOn;
        double _shiftLaserOff;

        public override void Load(CommandList commands)
        {
            _commands = commands;
            commands.Clear();

            AddFileHeader(commands);
            commands.Add(new GxxCommand() { GCodeAdd = "; File=" + LoadOptions.FileName });
            commands.Add(new GxxCommand() { GCodeAdd = "; LaserSize=" + LoadOptions.LaserSize.ToString() });
            commands.Add(new GxxCommand() { GCodeAdd = "; LaserOnCommand=" + LoadOptions.PenDownCommandString });
            commands.Add(new GxxCommand() { GCodeAdd = "; LaserOffCommand=" + LoadOptions.PenUpCommandString });

            _shiftX = (double)LoadOptions.LaserSize / 2.0;
            _shiftY = (double)LoadOptions.LaserSize / 2.0;

            const double SHIFT = Math.PI / 8.0;

            _shiftLaserOn  = SHIFT * (double) LoadOptions.LaserSize;
            _shiftLaserOff = -SHIFT * (double)LoadOptions.LaserSize;

            using (System.Drawing.Bitmap bx = new System.Drawing.Bitmap(LoadOptions.FileName))
            {
                System.Drawing.Bitmap b;
                switch (bx.PixelFormat)
                {
                    case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                        b = bx;
                        break;

                    case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    case System.Drawing.Imaging.PixelFormat.Format32bppRgb:

                        commands.Add(new GxxCommand() { GCodeAdd = "; Image Converted with FloydSteinbergDither" } );
                        commands.Add(new GxxCommand() { GCodeAdd = "; GrayThreshold=" + LoadOptions.GrayThreshold.ToString() });
                        b = new Framework.Tools.Drawing.FloydSteinbergDither() { Graythreshold = LoadOptions.GrayThreshold }.Process(bx);
                        break;

                    default:
                        throw new ArgumentException("Bitmap.PixelFormat not supported");
                }

                _sizeX = b.Width;
                _sizeY = b.Height;
                _pixelSizeX = 25.4 / b.HorizontalResolution;
                _pixelSizeY = 25.4 / b.VerticalResolution;

                if (b.PixelFormat != System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
                    throw new ArgumentException("Bitmap must be Format1bbp");

				commands.Add(new GxxCommand() { GCodeAdd = "; Image.Width="  + _sizeX.ToString() });
				commands.Add(new GxxCommand() { GCodeAdd = "; Image.Height=" + _sizeY.ToString() });
				commands.Add(new GxxCommand() { GCodeAdd = "; Image.HorizontalResolution(DPI)=" + b.HorizontalResolution.ToString() });
				commands.Add(new GxxCommand() { GCodeAdd = "; Image.VerticalResolution(DPI)=" + b.VerticalResolution.ToString() });

                commands.Add(new GxxCommand() { GCodeAdd = "; Speed=" + LoadOptions.PenMoveSpeed.ToString() });

                if (LoadOptions.PenMoveSpeed.HasValue)
                {
                    var setspeed = new G01Command();
                    setspeed.AddVariable('F', LoadOptions.PenMoveSpeed.Value);
                    _commands.Add(setspeed);
                }

                _laserOn = true;
                LaserOff();
                int black = System.Drawing.Color.Black.ToArgb();

                for (int y = 0; y < _sizeY; y++)
                {
                    var homec = new G00Command();
                    homec.AddVariable('X', (decimal) Math.Round(0 * _pixelSizeX + _shiftX, 2));
                    homec.AddVariable('Y', (decimal) Math.Round((_sizeY-y-1) * _pixelSizeY + _shiftY, 2));
                    _commands.Add(homec);
                    bool wasLaserOn = true;
                    bool lastLaserOn = false;

                    for (int x = 0; x < _sizeX; x++)
                    {
                        var col = b.GetPixel(x, y);

                        bool isLaserOn = col.ToArgb() == black;

                        if (isLaserOn != wasLaserOn && x != 0)
                        {
                            AddCommandX(x - 1,wasLaserOn);
                            wasLaserOn = isLaserOn;
                        }
                        else if (x==0)
                        {
                            wasLaserOn = isLaserOn;
                        }
                        lastLaserOn = isLaserOn;

                        if (isLaserOn)
                            LaserOn();
                        else
                            LaserOff();
                    }
                    if (lastLaserOn)
                        AddCommandX(_sizeX, wasLaserOn);

                    LaserOff();
                }
            }
            commands.UpdateCache();
        }

        private void AddCommandX(int x, bool laserOn)
        {
            // start laser a bit later but switch it off earlier
            double shift = 0;
            shift = laserOn ? _shiftLaserOff : _shiftLaserOn;

            var c = new G01Command();
            c.AddVariable('X', (decimal) Math.Round((x * _pixelSizeX) + _shiftX + shift, 2));
            _commands.Add(c);
        }

        private void LaserOn()
        {
            if (!_laserOn)
            {
                _commands.Add(CommandFactory.CreateOrDefault(LoadOptions.PenDownCommandString));
                _laserOn = true;
            }
        }
        private void LaserOff()
        {
            if (_laserOn)
            {
                _commands.Add(CommandFactory.CreateOrDefault(LoadOptions.PenUpCommandString));
                _laserOn = false;
            }
        }
    }
}

