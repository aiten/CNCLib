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

using System;
using System.Collections.Generic;
using System.Linq;

using CNCLib.Logic.Abstraction.DTO;

using Framework.Drawing;

namespace CNCLib.GCode.Generate.Load
{
    public partial class LoadHpgl
    {
        private class AutoScale
        {
            public LoadOptions LoadOptions { get; set; }
            public LoadHpgl    LoadX       { get; set; }

            public void AutoScaleList(IList<HpglCommand> list)
            {
                LoadX.AddComment("AutoScaleX", LoadX.LoadOptions.AutoScaleSizeX);
                LoadX.AddComment("AutoScaleY", LoadX.LoadOptions.AutoScaleSizeY);

                LoadX.AddComment("AutoScaleDistX", LoadX.LoadOptions.AutoScaleBorderDistX);
                LoadX.AddComment("AutoScaleDistY", LoadX.LoadOptions.AutoScaleBorderDistY);

                LoadX.AddComment("AutoScaleCenter", LoadX.LoadOptions.AutoScaleCenter.ToString());

                var minPt = new Point3D
                {
                    X = list.Where(x => x.IsPenCommand).Min(c => c.PointTo.X),
                    Y = list.Where(x => x.IsPenCommand).Min(c => c.PointTo.Y)
                };
                var maxPt = new Point3D
                {
                    X = list.Where(x => x.IsPenCommand).Max(c => c.PointTo.X),
                    Y = list.Where(x => x.IsPenCommand).Max(c => c.PointTo.Y)
                };

                decimal sizeX = (decimal)(maxPt.X0 - minPt.X0);
                decimal sizeY = (decimal)(maxPt.Y0 - minPt.Y0);

                decimal borderX = LoadX.LoadOptions.AutoScaleBorderDistX;
                decimal borderY = LoadX.LoadOptions.AutoScaleBorderDistY;

                decimal destSizeX = LoadX.LoadOptions.AutoScaleSizeX - 2m * borderX;
                decimal destSizeY = LoadX.LoadOptions.AutoScaleSizeY - 2m * borderY;

                LoadX.LoadOptions.ScaleX = destSizeX / sizeX;
                LoadX.LoadOptions.ScaleY = destSizeY / sizeY;

                if (LoadX.LoadOptions.AutoScaleKeepRatio)
                {
                    LoadX.LoadOptions.ScaleX = LoadX.LoadOptions.ScaleY = Math.Min(LoadX.LoadOptions.ScaleX, LoadX.LoadOptions.ScaleY);

                    if (LoadX.LoadOptions.AutoScaleCenter)
                    {
                        decimal sizeXscaled = LoadX.LoadOptions.ScaleX * sizeX;
                        decimal sizeYscaled = LoadX.LoadOptions.ScaleY * sizeY;

                        borderX += (destSizeX - sizeXscaled) / 2m;
                        borderY += (destSizeY - sizeYscaled) / 2m;
                    }
                }

                LoadX.LoadOptions.OfsX = -((decimal)(minPt.X0) - borderX / LoadX.LoadOptions.ScaleX);
                LoadX.LoadOptions.OfsY = -((decimal)(minPt.Y0) - borderY / LoadX.LoadOptions.ScaleY);
            }
        }
    }
}