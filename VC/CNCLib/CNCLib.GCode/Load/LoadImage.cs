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
        double _pixelSizeX = 1;
        double _pixelSizeY = 1;
        int _sizeX;
        int _sizeY;
        double _shiftX = 0;
        double _shiftY = 0;
        double _shiftLaserOn;
        double _shiftLaserOff;

        public override void Load()
        {
            PreLoad();

            AddComment("File" , LoadOptions.FileName );
			AddCommentForLaser();

            _shiftX = (double)LoadOptions.LaserSize / 2.0;
            _shiftY = (double)LoadOptions.LaserSize / 2.0;

            //            const double SHIFT = Math.PI / 16.0;
            //            const double SHIFT = Math.PI / 8.0;
            const double SHIFT = 0;

            _shiftLaserOn = -SHIFT * (double) LoadOptions.LaserSize;
            _shiftLaserOff = SHIFT * (double)LoadOptions.LaserSize;

            using (System.Drawing.Bitmap bx = new System.Drawing.Bitmap(LoadOptions.FileName))
            {
                System.Drawing.Bitmap b;
                switch (bx.PixelFormat)
                {
                    case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                        if(LoadOptions.AutoScale)
                            b = ConvertImage(bx);
                        else
                            b = bx;
                        break;

                    case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    case System.Drawing.Imaging.PixelFormat.Format24bppRgb:

                        b = ConvertImage(bx);
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

                b.Save(LoadOptions.ImageWriteToFileName, System.Drawing.Imaging.ImageFormat.Bmp);

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

        private System.Drawing.Bitmap ConvertImage(System.Drawing.Bitmap bx)
        {
            System.Drawing.Bitmap b = bx;
            decimal scaleX = LoadOptions.ScaleX;
            decimal scaleY = LoadOptions.ScaleY;
            double dpiX;
            double dpiY;

            if (LoadOptions.ImageDPIX.HasValue)
                dpiX = (double)LoadOptions.ImageDPIX.Value;
            else
                dpiX = b.HorizontalResolution;

            if (LoadOptions.ImageDPIY.HasValue)
                dpiY = (double)LoadOptions.ImageDPIX.Value;
            else
                dpiY = b.HorizontalResolution;


            if (LoadOptions.AutoScale)
            {
                AddComment("AutoScaleX" , LoadOptions.AutoScaleSizeX);
                AddComment("AutoScaleY" , LoadOptions.AutoScaleSizeY);
                AddComment("DPI_X" , dpiX);
                AddComment("DPI_Y" , dpiY);
                double nowX = (double)b.Width;
                double nowY = (double)b.Height;
                double newX = ((double)LoadOptions.AutoScaleSizeX) * dpiX / 25.4;
                double newY = ((double)LoadOptions.AutoScaleSizeY) * dpiY / 25.4;
                scaleX = (decimal)(newX / nowX);
                scaleY = (decimal)(newY / nowY);
                LoadOptions.ScaleX = scaleX;
                LoadOptions.ScaleY = scaleY;
            }

            if (scaleX != 1.0m)
            {
                AddComment("ScaleX" , scaleX);
                AddComment("ScaleY" , scaleY);
                b = Framework.Tools.Drawing.ImageHelper.ScaleTo(bx, (int)(b.Width * scaleX), (int)(b.Height * scaleY));
                b.SetResolution((float)dpiX, (float)dpiY);
            }

            switch (LoadOptions.Dither)
            {
                case LoadInfo.DitherFilter.FloydSteinbergDither:
                    AddComment("Image Converted with FloydSteinbergDither");
                    AddComment("GrayThreshold" , LoadOptions.GrayThreshold);
                    b = new Framework.Tools.Drawing.FloydSteinbergDither() { Graythreshold = LoadOptions.GrayThreshold }.Process(b);
                    break;
                case LoadInfo.DitherFilter.NewspaperDither:
                    AddComment("Image Converted with NewspaperDither" );
                    AddComment("GrayThreshold" , LoadOptions.GrayThreshold);
                    AddComment("Dithersize" , LoadOptions.NewspaperDitherSize);
                    b = new Framework.Tools.Drawing.NewspapergDither() { Graythreshold = LoadOptions.GrayThreshold, DotSize = LoadOptions.NewspaperDitherSize }.Process(b);
                    break;
            }

            return b;
        }

        private void WriteGCode(System.Drawing.Bitmap b)
        {
            ForceLaserOff();
            int black = System.Drawing.Color.Black.ToArgb();
            int lasty = -1;

            for (int y = 0; y < _sizeY; y++)
            {
                bool wasLaserOn = true;
                bool lastLaserOn = false;

                for (int x = 0; x < _sizeX; x++)
                {
                    var col = b.GetPixel(x, y);

                    bool isLaserOn = col.ToArgb() == black;

                    if (isLaserOn != wasLaserOn && x != 0)
                    {
                        AddCommandX(x - 1, y, ref lasty, wasLaserOn);
                        wasLaserOn = isLaserOn;
                    }
                    else if (x == 0)
                    {
                        wasLaserOn = isLaserOn;
                        if (isLaserOn)
                            AddCommandX(x, y, ref lasty, wasLaserOn);
                    }
                    lastLaserOn = isLaserOn;

                    if (isLaserOn)
                        LaserOn();
                    else
                        LaserOff();
                }
                if (lastLaserOn)
                    AddCommandX(_sizeX, y, ref lasty, wasLaserOn);

                LaserOff();
            }
        }

        private void AddCommandX(int x, int y, ref int lasty, bool laserOn)
        {
            // start laser a bit later but switch it off earlier
            double shift = 0;
            shift = laserOn ? _shiftLaserOff : _shiftLaserOn;

            if (y != lasty)
            {
                var cy = new G00Command();
                int x1 = x - 4; if (x1 < 0) x = 0;

                cy.AddVariable('X', (decimal)Math.Round((x1 * _pixelSizeX) + _shiftX + shift, 2));
                cy.AddVariable('Y', (decimal)Math.Round((_sizeY - y - 1) * _pixelSizeY + _shiftY, 2));
                lasty = y;
                Commands.Add(cy);
            }

            var cx = new G01Command();
            cx.AddVariable('X', (decimal) Math.Round((x * _pixelSizeX) + _shiftX + shift, 2));
            Commands.Add(cx);
        }
    }
}

