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

namespace Framework.Drawing
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
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Color currentPixel = GetPixel(x, y);
                    currentPixel.Saturation();

                    //Color bestColorRGB = FindNearestColorBW(currentPixel);
                    Color bestColorRGB = FindNearestColorGrayScale(currentPixel);
                    SetPixel(x, y, bestColorRGB);

                    int errorR = (currentPixel.R) - (bestColorRGB.R);
                    int errorG = (currentPixel.G) - (bestColorRGB.G);
                    int errorB = (currentPixel.B) - (bestColorRGB.B);

                    if (x + 1 < Width)
                    {
                        AddPixelSaturation(x + 1, y + 0, (errorR * 7) / 16, (errorG * 7) / 16, (errorB * 7) / 16, 0);
                    }

                    if (y + 1 < Height)
                    {
                        if (x - 1 >= 0)
                        {
                            AddPixelSaturation(x - 1, y + 1, (errorR * 3) / 16, (errorG * 3) / 16, (errorB * 3) / 16, 0);
                        }

                        AddPixelSaturation(x + 0, y + 1, (errorR * 5) / 16, (errorG * 5) / 16, (errorB * 5) / 16, 0);
                        if (x + 1 < Width)
                        {
                            AddPixelSaturation(x + 1, y + 1, (errorR * 1) / 16, (errorG * 1) / 16, (errorB * 1) / 16, 0);
                        }
                    }
                }
            }
        }
    }

    #endregion
}