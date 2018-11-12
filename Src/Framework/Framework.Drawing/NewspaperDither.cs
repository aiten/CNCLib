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