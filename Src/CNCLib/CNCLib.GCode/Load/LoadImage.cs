////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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
using CNCLib.Logic.Contracts.DTO;
using Framework.Tools.Helpers;

namespace CNCLib.GCode.Load
{
    public class LoadImage : LoadImageBase
    {
        double _shiftLaserOn;
        double _shiftLaserOff;

        public override void Load()
        {
            PreLoad();

            AddCommentForLaser();

            //            const double SHIFT = Math.PI / 16.0;
            //            const double SHIFT = Math.PI / 8.0;
            const double SHIFT = 0;

            _shiftLaserOn  = -SHIFT * (double) LoadOptions.LaserSize;
            _shiftLaserOff = SHIFT * (double) LoadOptions.LaserSize;

            using (var bx = new System.Drawing.Bitmap(IOHelper.ExpandEnvironmentVariables(LoadOptions.FileName)))
            {
                System.Drawing.Bitmap b;
                switch (bx.PixelFormat)
                {
                    case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                    case System.Drawing.Imaging.PixelFormat.Format4bppIndexed:
                        b = ScaleImage(bx);
                        break;

                    case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    case System.Drawing.Imaging.PixelFormat.Format24bppRgb:

                        b = ConvertImage(ScaleImage(bx));
                        break;

                    default: throw new ArgumentException("Bitmap.PixelFormat not supported");
                }

                if (b.PixelFormat != System.Drawing.Imaging.PixelFormat.Format1bppIndexed && b.PixelFormat != System.Drawing.Imaging.PixelFormat.Format4bppIndexed)
                {
                    throw new ArgumentException("Bitmap must be Format1bbp");
                }

                WriteGCode(b);
            }

            PostLoad();
        }

        private System.Drawing.Bitmap ConvertImage(System.Drawing.Bitmap bx)
        {
            System.Drawing.Bitmap b = bx;

            switch (LoadOptions.Dither)
            {
                case LoadOptions.DitherFilter.FloydSteinbergDither:
                    AddComment("Image Converted with FloydSteinbergDither");
                    AddComment("GrayThreshold", LoadOptions.GrayThreshold);
                    b = new Framework.Tools.Drawing.FloydSteinbergDither { Graythreshold = LoadOptions.GrayThreshold }.Process(b);
                    break;
                case LoadOptions.DitherFilter.NewspaperDither:
                    AddComment("Image Converted with NewspaperDither");
                    AddComment("GrayThreshold", LoadOptions.GrayThreshold);
                    AddComment("Dithersize",    LoadOptions.NewspaperDitherSize);
                    b = new Framework.Tools.Drawing.NewspapergDither
                    {
                        Graythreshold = LoadOptions.GrayThreshold,
                        DotSize       = LoadOptions.NewspaperDitherSize
                    }.Process(b);
                    break;
            }

            return b;
        }

        protected override void WriteGCode()
        {
            ForceLaserOff();
            int black = System.Drawing.Color.Black.ToArgb();
            int lasty = -1;

            for (int y = 0; y < SizeY; y++)
            {
                bool wasLaserOn  = true;
                bool lastLaserOn = false;

                for (int x = 0; x < SizeX; x++)
                {
                    var col = Bitmap.GetPixel(x, y);

                    bool isLaserOn = col.ToArgb() == black;

                    if (LoadOptions.ImageInvert)
                    {
                        isLaserOn = !isLaserOn;
                    }

                    if (isLaserOn != wasLaserOn && x != 0)
                    {
                        AddCommandX(x, y, ref lasty, wasLaserOn);
                        wasLaserOn = isLaserOn;
                    }
                    else if (x == 0)
                    {
                        wasLaserOn = isLaserOn;
                        if (isLaserOn)
                        {
                            AddCommandX(x, y, ref lasty, false);
                        }
                    }

                    lastLaserOn = isLaserOn;

                    if (isLaserOn)
                    {
                        LaserOn();
                    }
                    else
                    {
                        LaserOff();
                    }
                }

                if (lastLaserOn)
                {
                    AddCommandX(SizeX, y, ref lasty, wasLaserOn);
                }

                LaserOff();
            }
        }

        private void AddCommandX(int x, int y, ref int lasty, bool laserOn)
        {
            // start laser a bit later but switch it off earlier
            double shift = laserOn ? _shiftLaserOff : _shiftLaserOn;

            if (y != lasty)
            {
                var    cy = new G00Command();
                double x1 = (x * PixelSizeX) + ShiftX + shift - (double) LoadOptions.LaserAccDist;

                cy.AddVariable('X', ToGCode(x1));
                cy.AddVariable('Y', ToGCode((SizeY - y - 1) * PixelSizeY + ShiftY));
                lasty = y;
                Commands.Add(cy);
            }

            Command cx;
            // if we have no laser on/off we switch with g01 and g00
            bool use_g1 = HaveLaserOnOffCommand() || laserOn;
            if (use_g1)
            {
                cx = new G01Command();
            }
            else
            {
                cx = new G00Command();
            }

            cx.AddVariable('X', ToGCode((x * PixelSizeX) + ShiftX + shift));

            if (!use_g1)
            {
                cx.AddVariableNoValue('F');
            }

            Commands.Add(cx);
        }
    }
}