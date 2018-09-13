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

using System.Drawing;
using System.Drawing.Imaging;
using FluentAssertions;
using Framework.Tools.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Framework.Test.Drawing
{
    [TestClass]
    public class DitherTest
    {
        public class DitherTestClass : DitherBase
        {
            protected override void ConvertImage()
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        //Color currentPixel = GetPixel(x, y);

                        if ((x + y) % 2 == 0)
                        {
                            SetPixel(x, y, 255, 255, 255, 255);
                        }
                        else
                        {
                            SetPixel(x, y, 0, 0, 0, 255);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ReadWriteImage()
        {
            var dt = new DitherTestClass();

            const int XSIZE = 100;
            const int YSIZE = 100;

            var b1 = new Bitmap(XSIZE, YSIZE);
            var b2 = dt.Process(b1);

            for (int x = 0; x < XSIZE; x++)
            {
                for (int y = 0; y < YSIZE; y++)
                {
                    Color col = b2.GetPixel(x, y);

                    if ((x + y) % 2 == 0)
                    {
                        col.R.Should().Be(255);
                        col.G.Should().Be(255);
                        col.B.Should().Be(255);
                    }
                    else
                    {
                        col.R.Should().Be(0);
                        col.G.Should().Be(0);
                        col.B.Should().Be(0);
                    }
                }
            }
        }


        [TestMethod]
        public void FloydSteinberg1()
        {
            var dt = new FloydSteinbergDither();

            const int XSIZE = 100;
            const int YSIZE = 100;

            var b1 = new Bitmap(XSIZE, YSIZE);

            b1.SetPixel(50, 50, Color.FromArgb(255, 255, 255, 255));

            var b2 = dt.Process(b1);

            for (int x = 0; x < XSIZE; x++)
            {
                for (int y = 0; y < YSIZE; y++)
                {
                    Color col = b2.GetPixel(x, y);

                    if (col.R != 0)
                    {
                        col.R.Should().Be(255);
                        col.G.Should().Be(255);
                        col.B.Should().Be(255);

                        x.Should().Be(50);
                        y.Should().Be(50);
                    }
                    else
                    {
                        col.R.Should().Be(0);
                        col.G.Should().Be(0);
                        col.B.Should().Be(0);
                    }
                }
            }
        }

        [TestMethod]
        public void FloydSteinberg2()
        {
            var dt = new FloydSteinbergDither();

            const int XSIZE = 100;
            const int YSIZE = 100;

            var b1 = new Bitmap(XSIZE, YSIZE, PixelFormat.Format32bppArgb);

            b1.SetPixel(50, 50, Color.FromArgb(255, 127, 127, 127));
            b1.SetPixel(50, 51, Color.FromArgb(255, 127, 127, 127));
            b1.SetPixel(50, 52, Color.FromArgb(255, 127, 127, 127));
            b1.SetPixel(50, 53, Color.FromArgb(255, 127, 127, 127));

            var b2 = dt.Process(b1);

            for (int x = 0; x < XSIZE; x++)
            {
                for (int y = 0; y < YSIZE; y++)
                {
                    Color col = b2.GetPixel(x, y);

                    if (col.R != 0)
                    {
                        col.R.Should().Be(255);
                        col.G.Should().Be(255);
                        col.B.Should().Be(255);

                        x.Should().Be(50);
                        y.Should().BeOneOf(53, 51);
                    }
                    else
                    {
                        col.R.Should().Be(0);
                        col.G.Should().Be(0);
                        col.B.Should().Be(0);
                    }
                }
            }
        }
    }
}