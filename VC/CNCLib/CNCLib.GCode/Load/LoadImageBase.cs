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
    public abstract class LoadImageBase : LoadBase
    {
        protected double PixelSizeX { get; private set; } = 1;
        protected double PixelSizeY { get; private set; } = 1;
        protected int SizeX { get; private set; }
        protected int SizeY { get; private set; }
        protected double ShiftX { get; private set; } = 0;
        protected double SiftY { get; private set; } = 0;
        protected System.Drawing.Bitmap Bitmap { get; private set; }

        protected void WriteGCode(System.Drawing.Bitmap b)
        {
            Bitmap = b;
            SizeX = b.Width;
            SizeY = b.Height;
            PixelSizeX = 25.4 / b.HorizontalResolution;
            PixelSizeY = 25.4 / b.VerticalResolution;

            ShiftX = (double)LoadOptions.LaserSize / 2.0;
            SiftY = (double)LoadOptions.LaserSize / 2.0;

            b.Save(LoadOptions.ImageWriteToFileName, System.Drawing.Imaging.ImageFormat.Bmp);

            AddComment("Image.Width", b.Width);
            AddComment("Image.Height", b.Width);
            AddComment("Image.HorizontalResolution(DPI)", b.HorizontalResolution);
            AddComment("Image.VerticalResolution(DPI)", b.VerticalResolution);

            if (LoadOptions.MoveSpeed.HasValue)
            {
                var setspeed = new G01Command();
                setspeed.AddVariable('F', LoadOptions.MoveSpeed.Value);
                Commands.Add(setspeed);
            }

            AddComment("ImageInvert", LoadOptions.ImageInvert.ToString());

            WriteGCode();
        }

        protected abstract void WriteGCode();

        protected System.Drawing.Bitmap ScaleImage(System.Drawing.Bitmap bx)
        {
            System.Drawing.Bitmap b = bx;
            decimal scaleX = LoadOptions.ScaleX;
            decimal scaleY = LoadOptions.ScaleY;
            double dpiX;
            double dpiY;

            dpiX = (double)(LoadOptions.ImageDPIX ?? (decimal)b.HorizontalResolution);
            dpiY = (double)(LoadOptions.ImageDPIY ?? (decimal)b.VerticalResolution);

            if (LoadOptions.AutoScale)
            {
                AddComment("AutoScaleKeepRatio", LoadOptions.AutoScaleKeepRatio.ToString());
                AddComment("AutoScaleX", LoadOptions.AutoScaleSizeX);
                AddComment("AutoScaleY", LoadOptions.AutoScaleSizeY);
                AddComment("DPI_X", dpiX);
                AddComment("DPI_Y", dpiY);
                double nowX = (double)b.Width;
                double nowY = (double)b.Height;
                double newX = ((double)LoadOptions.AutoScaleSizeX) * dpiX / 25.4;
                double newY = ((double)LoadOptions.AutoScaleSizeY) * dpiY / 25.4;
                scaleX = (decimal)(newX / nowX);
                scaleY = (decimal)(newY / nowY);
                if (LoadOptions.AutoScaleKeepRatio)
                {
                    scaleX = scaleY = Math.Min(scaleX, scaleY);
                }
                LoadOptions.ScaleX = scaleX;
                LoadOptions.ScaleY = scaleY;
            }

            if (scaleX != 1.0m || scaleY != 1.0m)
            {
                AddComment("ScaleX", scaleX);
                AddComment("ScaleY", scaleY);
                b = Framework.Tools.Drawing.ImageHelper.ScaleTo(bx, (int)(b.Width * scaleX), (int)(b.Height * scaleY));
                b.SetResolution((float)dpiX, (float)dpiY);
            }

            return b;
        }

    }
}

