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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Tools.Drawing
{
    public class FloydSteinbergDither : DitherBase
	{
        #region private members

        #endregion

        #region properties


        #endregion

        #region public


        #endregion

        #region private helper

		protected override void ConvertImage()
		{
			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
                    Color currentPixel = GetPixel(x, y);
					currentPixel.Saturation();
					Color bestColorRGB = FindNearestColorGrayScale(currentPixel);
                    SetPixel(x, y, bestColorRGB);

					int errorR = (currentPixel.R) - (bestColorRGB.R);
					int errorG = (currentPixel.G) - (bestColorRGB.G);
					int errorB = (currentPixel.B) - (bestColorRGB.B);

                    if (x + 1 < _width)
					{
                        AddPixel(x+1, y+0, (errorR * 7) >> 4, (errorG * 7) >> 4, (errorB * 7) >> 4, 0);
					}
					if (y + 1 < _height)
					{
						if (x - 1 > 0)
						{
							AddPixel(x - 1, y + 1, (errorR * 3) >> 4, (errorG * 3) >> 4, (errorB * 3) >> 4, 0);
						}
						AddPixel(x + 0, y + 1, (errorR * 5) >> 4, (errorG * 5) >> 4, (errorB * 5) >> 4, 0);
						if (x + 1 < _width)
						{
							AddPixel(x + 1, y + 1, (errorR * 1) >> 4, (errorG * 1) >> 4, (errorB * 1) >> 4, 0);
						}
					}
				}
			}
		}
	}

    #endregion
}
