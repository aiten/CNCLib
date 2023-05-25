/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace CNCLib.GCode.Generate.Load;

using System;

using CNCLib.GCode.Generate.Commands;
using CNCLib.Logic.Abstraction.DTO;

using Framework.Drawing;
using Framework.Tools;

using SkiaSharp;

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

        _shiftLaserOn  = -SHIFT * (double)LoadOptions.LaserSize;
        _shiftLaserOff = SHIFT * (double)LoadOptions.LaserSize;

        using (var bitmap = ImageHelperExtensions.LoadFromFile(IOHelper.ExpandEnvironmentVariables(LoadOptions.FileName)))
        {
            var bitmapScaled    = ScaleImage(bitmap);
            var bitmapConverted = ConvertImage(bitmapScaled);

            WriteGCode(bitmapConverted);
        }

        PostLoad();
    }

    private SKBitmap ConvertImage(SKBitmap bx)
    {
        SKBitmap b = bx;

        switch (LoadOptions.Dither)
        {
            case LoadOptions.DitherFilter.FloydSteinbergDither:
                AddComment("Image Converted with FloydSteinbergDither");
                AddComment("GrayThreshold", LoadOptions.GrayThreshold);
                b = new FloydSteinbergDither { GrayThreshold = LoadOptions.GrayThreshold }.Process(b);
                break;
            case LoadOptions.DitherFilter.NewspaperDither:
                AddComment("Image Converted with NewspaperDither");
                AddComment("GrayThreshold", LoadOptions.GrayThreshold);
                AddComment("Dithersize",    LoadOptions.NewspaperDitherSize);
                b = new NewspaperDither
                {
                    GrayThreshold = LoadOptions.GrayThreshold,
                    DotSize       = LoadOptions.NewspaperDitherSize
                }.Process(b);
                break;
        }

        return b;
    }

    protected override void WriteGCode()
    {
        ForceLaserOff();
        var black = SKColors.Black;
        int lastY = -1;

        for (int y = 0; y < SizeY; y++)
        {
            bool wasLaserOn  = true;
            bool lastLaserOn = false;

            for (int x = 0; x < SizeX; x++)
            {
                var col = Bitmap.GetPixel(x, y);

                bool isLaserOn = col == black;

                if (LoadOptions.ImageInvert)
                {
                    isLaserOn = !isLaserOn;
                }

                if (isLaserOn != wasLaserOn && x != 0)
                {
                    AddCommandX(x, y, ref lastY, wasLaserOn);
                    wasLaserOn = isLaserOn;
                }
                else if (x == 0)
                {
                    wasLaserOn = isLaserOn;
                    if (isLaserOn)
                    {
                        AddCommandX(x, y, ref lastY, false);
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
                AddCommandX(SizeX, y, ref lastY, wasLaserOn);
            }

            LaserOff();
        }
    }

    private void AddCommandX(int x, int y, ref int lastY, bool laserOn)
    {
        // start laser a bit later but switch it off earlier
        double shift = laserOn ? _shiftLaserOff : _shiftLaserOn;

        if (y != lastY)
        {
            var    cy = new G00Command();
            double x1 = (x * PixelSizeX) + ShiftX + shift - (double)LoadOptions.LaserAccDist;

            cy.AddVariable('X', ToGCode(x1));
            cy.AddVariable('Y', ToGCode((SizeY - y - 1) * PixelSizeY + ShiftY));
            lastY = y;
            Commands.Add(cy);
        }

        Command cx;

        // if we have no laser on/off we switch with g01 and g00
        bool useG1 = HaveLaserOnOffCommand() || laserOn;
        if (useG1)
        {
            cx = new G01Command();
        }
        else
        {
            cx = new G00Command();
        }

        cx.AddVariable('X', ToGCode((x * PixelSizeX) + ShiftX + shift));

        if (!useG1)
        {
            cx.AddVariableNoValue('F');
        }

        Commands.Add(cx);
    }
}