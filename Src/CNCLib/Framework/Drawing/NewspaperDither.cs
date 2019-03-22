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
    public class NewspaperDither : FloydSteinbergDither
    {
        #region private members

        #endregion

        #region properties

        public int DotSize { get; set; } = 5;

        #endregion

        #region public

        #endregion

        #region private helper

        protected override void ConvertImage()
        {
            base.ConvertImage();

            for (int y = 0; y < Height; y += DotSize)
            {
                for (int x = 0; x < Width; x += DotSize)
                {
                    int count = CountBlack(x, y);
                    FillBlack(x, y, count);
                }
            }
        }

        private int CountBlack(int x, int y)
        {
            int count = 0;
            for (int iy = 0; iy < DotSize; iy++)
            {
                for (int ix = 0; ix < DotSize; ix++)
                {
                    if (IsPixel(x + ix, y + iy))
                    {
                        Color currentPixel = GetPixel(x + ix, y + iy);
                        if (currentPixel.R == 0)
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        struct Offsets
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        private readonly Offsets[] _offsets = new[]
        {
            new Offsets { X = 0, Y  = 0 }, new Offsets { X = -1, Y = 0 }, new Offsets { X = 0, Y = 1 }, new Offsets { X = 1, Y = 0 }, new Offsets { X = 0, Y = -1 },
            new Offsets { X = -1, Y = 1 },
            new Offsets { X = 1, Y  = 1 }, new Offsets { X = 1, Y = -1 }, new Offsets { X = -1, Y = -1 }, new Offsets { X = -2, Y = 0 }, new Offsets { X = 0, Y = 2 },
            new Offsets { X = 2, Y  = 0 },
            new Offsets { X = 0, Y  = -2 }, new Offsets { X = -2, Y = 1 }, new Offsets { X = 1, Y = 2 }, new Offsets { X = 2, Y = -1 }, new Offsets { X = -1, Y = -2 },
            new Offsets { X = -1, Y = 2 },
            new Offsets { X = 2, Y  = 1 }, new Offsets { X = 1, Y = -2 }, new Offsets { X = -2, Y = -1 }, new Offsets { X = -2, Y = 2 }, new Offsets { X = 2, Y = 2 },
            new Offsets { X = 2, Y  = -2 },
            new Offsets { X = -2, Y = -2 }
        };

        private void FillBlack(int x, int y, int count)
        {
            var black = new Color { R = 0, G   = 0, B   = 0, A   = 0 };
            var white = new Color { R = 255, G = 255, B = 255, A = 255 };

            for (int i = 0; i < DotSize * DotSize; i++)
            {
                int xx = x + DotSize / 2 + _offsets[i].X;
                int yy = y + DotSize / 2 + _offsets[i].Y;

                if (IsPixel(xx, yy))
                {
                    if (count > 0)
                    {
                        count--;
                        SetPixel(xx, yy, black);
                    }
                    else
                    {
                        SetPixel(xx, yy, white);
                    }
                }
            }
        }
    }

    #endregion
}