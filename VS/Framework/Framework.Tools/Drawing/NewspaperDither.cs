////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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


namespace Framework.Tools.Drawing
{
	public class NewspapergDither : FloydSteinbergDither
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

            for (int y = 0; y < _height; y += DotSize)
            {
                for (int x = 0; x < _width; x += DotSize)
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
                    if (IsPixel(x + ix,y + iy))
                    {
                        Color currentPixel = GetPixel(x+ix, y+iy);
                        if (currentPixel.R == 0)
                            count++;
                    }
                }
            }
            return count;
        }

        struct Offsets
        {
            public int x;
            public int y;
        }

        Offsets[] _offsets = new Offsets[]
        {
            new Offsets() { x =0, y=0 },
            new Offsets() { x = -1, y = 0 },  new Offsets() { x = 0,  y = 1 }, new Offsets() { x = 1, y = 0 },  new Offsets() { x = 0,  y = -1 },
            new Offsets() { x = -1, y = 1 },  new Offsets() { x = 1,  y = 1 }, new Offsets() { x = 1, y = -1 }, new Offsets() { x = -1,  y = -1 },

            new Offsets() { x = -2, y = 0 },  new Offsets() { x = 0,  y = 2 }, new Offsets() { x = 2, y = 0 },    new Offsets() { x = 0,  y = -2 },
            new Offsets() { x = -2, y = 1 },  new Offsets() { x = 1,  y = 2 }, new Offsets() { x = 2, y = -1 },   new Offsets() { x = -1, y = -2 },
            new Offsets() { x = -1, y = 2 },  new Offsets() { x = 2,  y = 1 }, new Offsets() { x = 1, y = -2 },   new Offsets() { x = -2, y = -1 },
            new Offsets() { x = -2, y = 2 },  new Offsets() { x = 2,  y = 2 }, new Offsets() { x = 2, y = -2 },   new Offsets() { x = -2, y = -2 }
        };

        private void FillBlack(int x, int y, int count)
        {
            var black = new Color() { R = 0, G = 0, B = 0, A = 0 };
            var white = new Color() { R = 255, G = 255, B = 255, A = 255 };

            for (int i=0;i<DotSize*DotSize;i++)
            {
                int xx = x + DotSize / 2 + _offsets[i].x;
                int yy = y + DotSize / 2 + _offsets[i].y;

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
