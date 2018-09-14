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
using System.Drawing;
using System.Drawing.Imaging;

namespace Framework.Tools.Drawing
{
    public abstract class DitherBase
    {
        #region private members

        int   _bytesPerPixel = 4;
        int   _scansize;
        int[] _rgbValues;

        int          _addForA = 3;
        readonly int _addForR = 2;
        readonly int _addForG = 1;
        readonly int _addForB = 0;

        #endregion

        protected int Height { get; set; }
        protected int Width  { get; set; }

        protected struct Color
        {
            public int R { get; set; }
            public int G { get; set; }
            public int B { get; set; }
            public int A { get; set; }

            public Color Saturation()
            {
                A = Saturation(A);
                R = Saturation(R);
                G = Saturation(G);
                B = Saturation(B);
                return this;
            }

            public static byte Saturation(int r)
            {
                if (r <= 0)
                {
                    return 0;
                }

                if (r > 255)
                {
                    return 255;
                }

                return (byte) (r);
            }
        }

        #region properties

        public int Graythreshold { get; set; } = 127;

        #endregion

        #region public

        public Bitmap Process(Bitmap image)
        {
            ReadImage(image);

            ConvertImage();

            return WriteImage(image);
        }

        #endregion

        #region protected helper

        protected int Luminance(Color col)
        {
            // o..255 if RGB is Byte
            return (int) (0.2126 * col.R + 0.7152 * col.G + 0.0722 * col.B);
        }

        protected Color FindNearestColorBw(Color col)
        {
            int drb    = 255 - col.R;
            int dgb    = 255 - col.G;
            int dbb    = 255 - col.B;
            int errorb = drb * drb + dgb * dgb + dbb * dbb;

            int drw    = col.R;
            int dgw    = col.G;
            int dbw    = col.B;
            int errorw = drw * drw + dgw * dgw + dbw * dbw;

            if (errorw > errorb)
            {
                return new Color { R = 255, G = 255, B = 255, A = 255 };
            }

            return new Color { R = 0, G = 0, B = 0, A = 255 };
        }

        protected Color FindNearestColorGrayScale(Color col)
        {
            if (Luminance(col) > Graythreshold)
            {
                return new Color { R = 255, G = 255, B = 255, A = 255 };
            }

            return new Color { R = 0, G = 0, B = 0, A = 255 };
        }

        protected int ToByteIdx(int x, int y)
        {
            return y * _scansize + x * _bytesPerPixel;
        }

        protected bool IsPixel(int x, int y)
        {
            return x < Width && y < Height;
        }

        protected Color GetPixel(int x, int y)
        {
            int idx = ToByteIdx(x, y);
            return new Color
            {
                R = _rgbValues[idx + _addForR],
                G = _rgbValues[idx + _addForG],
                B = _rgbValues[idx + _addForB],
                A = (_addForA < 0) ? 255 : _rgbValues[idx + _addForA]
            };
        }

        protected void SetPixel(int x, int y, Color color)
        {
            int idx = ToByteIdx(x, y);
            _rgbValues[idx + _addForR] = color.R;
            _rgbValues[idx + _addForG] = color.G;
            _rgbValues[idx + _addForB] = color.B;

            if (_addForA >= 0)
            {
                _rgbValues[idx + _addForA] = color.A;
            }
        }

        protected void SetPixel(int x, int y, int r, int g, int b, int a)
        {
            SetPixel(x, y, new Color { R = r, G = g, B = b, A = a });
        }

        protected void AddPixel(int x, int y, int r, int g, int b, int a)
        {
            Color pixel = GetPixel(x, y);
            SetPixel(x, y, pixel.R + r, pixel.G + g, pixel.B + b, pixel.A + a);
        }

        protected void AddPixelSaturation(int x, int y, int r, int g, int b, int a)
        {
            Color pixel = GetPixel(x, y);
            SetPixel(x, y, Color.Saturation(pixel.R + r), Color.Saturation(pixel.G + g), Color.Saturation(pixel.B + b), Color.Saturation(pixel.A + a));
        }

        protected void ReadImage(Bitmap imageX)
        {
            Height = imageX.Height;
            Width  = imageX.Width;

            var        rect    = new Rectangle(0, 0, Width, Height);
            BitmapData bmpData = imageX.LockBits(rect, ImageLockMode.ReadOnly, imageX.PixelFormat);
            IntPtr     ptr     = bmpData.Scan0;
            _scansize = Math.Abs(bmpData.Stride);

            int bytes     = _scansize * Height;
            var rgbValues = new Byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            imageX.UnlockBits(bmpData);
            _rgbValues = new int[bytes];

            for (var i = 0; i < bytes; i++)
            {
                _rgbValues[i] = rgbValues[i];
            }

            switch (imageX.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    _bytesPerPixel = 3;
                    _addForA       = -1;
                    break;
            }
        }

        protected abstract void ConvertImage();

        protected Bitmap WriteImage(Bitmap imageX)
        {
            return WriteImageFormat4BppIndexed(imageX);
        }

        protected Bitmap WriteImageSamePixelFormat(Bitmap imageX)
        {
            var bsrc = new Bitmap(Width, Height, imageX.PixelFormat);

            var        rect    = new Rectangle(0, 0, Width, Height);
            BitmapData bmpData = bsrc.LockBits(rect, ImageLockMode.WriteOnly, bsrc.PixelFormat);
            IntPtr     ptr     = bmpData.Scan0;

            var rgbValues = new Byte[_rgbValues.Length];

            for (var i = 0; i < _rgbValues.Length; i++)
            {
                rgbValues[i] = Color.Saturation(_rgbValues[i]);
            }

            int bytes = Math.Abs(bmpData.Stride) * Height;
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bsrc.UnlockBits(bmpData);

            bsrc.SetResolution(imageX.HorizontalResolution, imageX.VerticalResolution);

            return bsrc;
        }

        protected Bitmap WriteImageFormat1BppIndexed(Bitmap bsrc)
        {
            var rect = new Rectangle(0, 0, Width, Height);

            var b = new Bitmap(Width, Height, PixelFormat.Format1bppIndexed);

            BitmapData bmpData   = b.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
            IntPtr     ptr       = bmpData.Scan0;
            int        scansize  = Math.Abs(bmpData.Stride);
            int        bytes     = scansize * Height;
            var        rgbvalues = new byte[bytes];

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    Color currentPixel = GetPixel(x, y);
                    if (currentPixel.R != 0)
                    {
                        rgbvalues[y * scansize + x / 8] += (byte) (0x80 >> (x % 8));
                    }
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbvalues, 0, ptr, bytes);
            b.UnlockBits(bmpData);

            b.SetResolution(bsrc.HorizontalResolution, bsrc.VerticalResolution);

            ColorPalette cp = b.Palette;
            cp.Entries[0] = System.Drawing.Color.Black;
            cp.Entries[1] = System.Drawing.Color.White;

            b.Palette = cp;

            return b;
        }

        protected Bitmap WriteImageFormat4BppIndexed(Bitmap bsrc)
        {
            var rect = new Rectangle(0, 0, Width, Height);

            var b = new Bitmap(Width, Height, PixelFormat.Format4bppIndexed);

            BitmapData bmpData   = b.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);
            IntPtr     ptr       = bmpData.Scan0;
            int        scansize  = Math.Abs(bmpData.Stride);
            int        bytes     = scansize * Height;
            var        rgbvalues = new byte[bytes];

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    Color currentPixel = GetPixel(x, y);
                    if (currentPixel.R != 0)
                    {
                        rgbvalues[y * scansize + x / 2] += (x % 2 == 0) ? (byte) 0xf0 : (byte) 0xf;
                    }
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbvalues, 0, ptr, bytes);
            b.UnlockBits(bmpData);

            b.SetResolution(bsrc.HorizontalResolution, bsrc.VerticalResolution);

            ColorPalette cp = b.Palette;

            for (var i = 0; i < 15; i++)
            {
                cp.Entries[i] = System.Drawing.Color.FromArgb(255, i * 16, i * 16, i * 16);
            }

            cp.Entries[15] = System.Drawing.Color.FromArgb(255, 255, 255, 255);

            b.Palette = cp;

            return b;
        }
    }

    #endregion
}