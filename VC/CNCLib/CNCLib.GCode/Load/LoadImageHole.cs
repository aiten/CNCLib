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
    public class LoadImageHole : LoadBase
    {
        double _pixelSizeX = 1;
        double _pixelSizeY = 1;
        int _sizeX;
        int _sizeY;
        double _shiftX = 0;
        double _shiftY = 0;

        public override void Load()
        {
            PreLoad();
            AddComment("File" , LoadOptions.FileName );
			AddCommentForLaser();

            _shiftX = (double)LoadOptions.LaserSize / 2.0;
            _shiftY = (double)LoadOptions.LaserSize / 2.0;

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
                    case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:

                        b = bx;
                        break;

                    default:
                        throw new ArgumentException("Bitmap.PixelFormat not supported");
                }

                _sizeX = b.Width;
                _sizeY = b.Height;

                _pixelSizeX = 25.4 / b.HorizontalResolution;
                _pixelSizeY = 25.4 / b.VerticalResolution;

                AddComment("Image.Width" , _sizeX);
                AddComment("Image.Height" , _sizeY);
                AddComment("Image.HorizontalResolution(DPI)" , b.HorizontalResolution);
                AddComment("Image.VerticalResolution(DPI)" , b.VerticalResolution);

                AddComment("Speed" , LoadOptions.MoveSpeed);

                if (LoadOptions.MoveSpeed.HasValue)
                {
                    var setspeed = new G01Command();
                    setspeed.AddVariable('F', LoadOptions.MoveSpeed.Value);
                    Commands.Add(setspeed);
                }

                WriteGCode(b);
            }
            Commands.UpdateCache();
        }

        protected Byte FindNearestColorGrayScale(Byte colorR, Byte colorG, Byte colorB)
        {
            return (Byte) (0.2126 * colorR + 0.7152 * colorG + 0.0722 * colorB);
        }

        private void WriteGCode(System.Drawing.Bitmap b)
        {
            LaserOff();

            for (int y = 0; y < _sizeY; y++)
            {
                for (int x = 0; x < _sizeX; x++)
                {
                    var col = b.GetPixel(x, y);
                    AddCommandX(x, y, FindNearestColorGrayScale(col.R, col.G, col.B));
                }

                LaserOff();
            }
        }

        private void AddCommandX(int x, int y, Byte size )
        {
            double holesize = (int) ((((size + 12) / 24) * 24)*0.7);    // int => rounc

            double hsizeX = (double) (holesize * _pixelSizeX / 256 / 2);
            double hsizeY = (double) (holesize * _pixelSizeY / 256 / 2);

            if (hsizeX > 0.1 && hsizeY > 0.1)
            {
                double xx = (double)Math.Round(((double)(x + 0.5) * _pixelSizeX), 2);
                double yy = (double)Math.Round(((double)(_sizeY - y - 0.5) * _pixelSizeY), 2);

                Command c = new G00Command();
                c.AddVariable('X', (decimal)Math.Round(xx - hsizeX + _shiftX, 2));
                c.AddVariable('Y', (decimal)Math.Round(yy - hsizeY + _shiftY, 2));
                Commands.Add(c);
                LaserOn();

                c = new G01Command();
                c.AddVariable('X', (decimal)Math.Round(xx + hsizeX + _shiftX, 2));
                c.AddVariable('Y', (decimal)Math.Round(yy - hsizeY + _shiftY, 2));
                Commands.Add(c);

                c = new G01Command();
                c.AddVariable('X', (decimal)Math.Round(xx + hsizeX + _shiftX, 2));
                c.AddVariable('Y', (decimal)Math.Round(yy + hsizeY + _shiftY, 2));
                Commands.Add(c);

                c = new G01Command();
                c.AddVariable('X', (decimal)Math.Round(xx - hsizeX + _shiftX, 2));
                c.AddVariable('Y', (decimal)Math.Round(yy + hsizeY + _shiftY, 2));
                Commands.Add(c);

                c = new G01Command();
                c.AddVariable('X', (decimal)Math.Round(xx - hsizeX + _shiftX, 2));
                c.AddVariable('Y', (decimal)Math.Round(yy - hsizeY + _shiftY, 2));
                Commands.Add(c);

                LaserOff();
            }
        }
    }
}

