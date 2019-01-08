////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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

using CNCLib.GCode.Commands;

using Framework.Drawing;

namespace CNCLib.GCode.Load
{
    public abstract class LoadImageBase : LoadBase
    {
        protected         double PixelSizeX { get; private set; } = 1;
        protected         double PixelSizeY { get; private set; } = 1;
        protected virtual double PixelDistX => 0;
        protected virtual double PixelDistY => 0;

        protected int                   SizeX  { get; private set; }
        protected int                   SizeY  { get; private set; }
        protected double                ShiftX { get; private set; } = 0;
        protected double                ShiftY { get; private set; } = 0;
        protected System.Drawing.Bitmap Bitmap { get; private set; }

        protected void WriteGCode(System.Drawing.Bitmap b)
        {
            Bitmap     = b;
            SizeX      = b.Width;
            SizeY      = b.Height;
            PixelSizeX = 25.4 / b.HorizontalResolution;
            PixelSizeY = 25.4 / b.VerticalResolution;

            ShiftX = (double)LoadOptions.LaserSize / 2.0;
            ShiftY = (double)LoadOptions.LaserSize / 2.0;

            if (!string.IsNullOrEmpty(LoadOptions.ImageWriteToFileName))
            {
                b.Save(Environment.ExpandEnvironmentVariables(LoadOptions.ImageWriteToFileName), System.Drawing.Imaging.ImageFormat.Bmp);
            }

            AddComment("Image.Width",                     b.Width);
            AddComment("Image.Height",                    b.Width);
            AddComment("Image.HorizontalResolution(DPI)", b.HorizontalResolution);
            AddComment("Image.VerticalResolution(DPI)",   b.VerticalResolution);
            AddComment("ImageInvert",                     LoadOptions.ImageInvert.ToString());

            if (LoadOptions.MoveSpeed.HasValue)
            {
                var setSpeed = new G01Command();
                setSpeed.AddVariable('F', LoadOptions.MoveSpeed.Value);
                Commands.Add(setSpeed);
            }

            WriteGCode();
        }

        protected abstract void WriteGCode();

        protected System.Drawing.Bitmap ScaleImage(System.Drawing.Bitmap bx)
        {
            System.Drawing.Bitmap b      = bx;
            decimal               scaleX = LoadOptions.ScaleX;
            decimal               scaleY = LoadOptions.ScaleY;

            var dpiX = (double)(LoadOptions.ImageDPIX ?? (decimal)b.HorizontalResolution);
            var dpiY = (double)(LoadOptions.ImageDPIY ?? (decimal)b.VerticalResolution);

            if (LoadOptions.AutoScale)
            {
                AddComment("AutoScaleKeepRatio", LoadOptions.AutoScaleKeepRatio.ToString());
                AddComment("AutoScaleX",         LoadOptions.AutoScaleSizeX);
                AddComment("AutoScaleY",         LoadOptions.AutoScaleSizeY);
                AddComment("DPI_X",              dpiX);
                AddComment("DPI_Y",              dpiY);
                double scaleDPIX = dpiX;
                double scaleDPIY = dpiY;
                if (PixelDistX > 0.0)
                {
                    double dotSize = 25.4 / scaleDPIX + PixelDistX;
                    scaleDPIX = 25.4 / dotSize;
                    AddComment("DPI_X(Dist)", scaleDPIX);
                }

                if (PixelDistY > 0.0)
                {
                    double dotSize = 25.4 / scaleDPIY + PixelDistY;
                    scaleDPIY = 25.4 / dotSize;
                    AddComment("DPI_Y(Dist)", scaleDPIY);
                }

                double nowX = (double)b.Width;
                double nowY = (double)b.Height;
                double newX = ((double)LoadOptions.AutoScaleSizeX) * scaleDPIX / 25.4;
                double newY = ((double)LoadOptions.AutoScaleSizeY) * scaleDPIY / 25.4;
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
                b = ImageHelper.ScaleTo(bx, (int)(b.Width * scaleX), (int)(b.Height * scaleY));
                b.SetResolution((float)dpiX, (float)dpiY);
            }

            return b;
        }
    }
}