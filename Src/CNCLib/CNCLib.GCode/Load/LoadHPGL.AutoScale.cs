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
using System.Collections.Generic;
using System.Linq;

using CNCLib.Logic.Contract.DTO;

using Framework.Drawing;

namespace CNCLib.GCode.Load
{
    public partial class LoadHPGL
    {
        private class AutoScale
        {
            public LoadOptions LoadOptions { get; set; }
            public LoadHPGL    LoadX       { get; set; }

            public void AutoScaleList(IList<HPGLCommand> list)
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