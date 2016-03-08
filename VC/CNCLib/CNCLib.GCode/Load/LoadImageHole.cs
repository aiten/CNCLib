////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
    public class LoadImageHole : LoadImageBase
    {
        public override void Load()
        {
            PreLoad();

            AddCommentForLaser();

            using (System.Drawing.Bitmap bx = new System.Drawing.Bitmap(LoadOptions.FileName))
            {
                System.Drawing.Bitmap b;
                switch (bx.PixelFormat)
                {
                    case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                        b = ScaleImage(bx);
                        break;

                    case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:

                        b = ScaleImage(bx);
                        break;

                    default:
                        throw new ArgumentException("Bitmap.PixelFormat not supported");
                }

                AddComment("ImageConvert", LoadOptions.ImageInvert.ToString());

                WriteGCode(b);
            }
            PostLoad();
        }
 
        protected override void WriteGCode()
        {
            LaserOff();

            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    var col = Bitmap.GetPixel(x, y);
                    AddCommandX(x, y, FindNearestColorGrayScale(col.R, col.G, col.B));
                }

                LaserOff();
            }
        }

        protected Byte FindNearestColorGrayScale(Byte colorR, Byte colorG, Byte colorB)
        {
            return (Byte) (0.2126 * colorR + 0.7152 * colorG + 0.0722 * colorB);
        }

        private void AddCommandX(int x, int y, Byte size )
        {
            const double minholesize = 0.25;

            if (LoadOptions.ImageInvert)
            {
                size = (Byte) (255 - size);
            }

//            double holesize = (int) ((((size + 12) / 24) * 24)*0.7);    // int => round
            double holesize = size*0.7;

            double hsizeX = (double) (holesize * PixelSizeX / 256 / 2);
            double hsizeY = (double) (holesize * PixelSizeY / 256 / 2);

            if (hsizeX > minholesize && hsizeY > minholesize)
            {
                double xx = (double)Math.Round(((double)(x + 0.5) * PixelSizeX), 2);
                double yy = (double)Math.Round(((double)(SizeY - y - 0.5) * PixelSizeY), 2);

                Command c = new G00Command();
                c.AddVariable('X', (decimal)Math.Round(xx - hsizeX + ShiftX, 2));
                c.AddVariable('Y', (decimal)Math.Round(yy - hsizeY + SiftY, 2));
                Commands.Add(c);
                LaserOn();

                c = new G01Command();
                c.AddVariable('X', (decimal)Math.Round(xx + hsizeX + ShiftX, 2));
                c.AddVariable('Y', (decimal)Math.Round(yy - hsizeY + SiftY, 2));
                Commands.Add(c);

                c = new G01Command();
                c.AddVariable('X', (decimal)Math.Round(xx + hsizeX + ShiftX, 2));
                c.AddVariable('Y', (decimal)Math.Round(yy + hsizeY + SiftY, 2));
                Commands.Add(c);

                c = new G01Command();
                c.AddVariable('X', (decimal)Math.Round(xx - hsizeX + ShiftX, 2));
                c.AddVariable('Y', (decimal)Math.Round(yy + hsizeY + SiftY, 2));
                Commands.Add(c);

                c = new G01Command();
                c.AddVariable('X', (decimal)Math.Round(xx - hsizeX + ShiftX, 2));
                c.AddVariable('Y', (decimal)Math.Round(yy - hsizeY + SiftY, 2));
                Commands.Add(c);

                LaserOff();
            }
        }
    }
}

