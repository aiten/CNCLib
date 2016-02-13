////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
    public abstract class DitherBase
	{
        #region private members

        protected int _bytesPerPixel = 4;
        protected int _height;
        protected int _width;

        int _AddForA = 3;
        int _AddForR = 2;
        int _AddForG = 1;
        int _AddForB = 0;

        #endregion

        protected struct Color
        {
            public Byte R;
            public Byte G;
            public Byte B;
            public Byte A;
        };

        #region properties

        public int Graythreshold { get; set; } = 127;

        #endregion

        #region public

        public Bitmap Process(Bitmap image)
        {
            byte[] rgbValues = ReadImage(image);

            ConvertImage(rgbValues);

            return WriteImage(rgbValues,image);
        }

        #endregion

        #region protected helper

        protected static byte Saturation(int r)
		{
			if (r <= 0)
				return 0;
			else if (r > 255)
				return (byte)255;
			else
				return (byte)(r);
		}

        protected bool FindNearestColorGrayScale(Byte colorR, Byte colorG, Byte colorB)
		{
			return (0.2126 * colorR + 0.7152 * colorG + 0.0722 * colorB) >= Graythreshold;
		}

        protected int ToByteIdx(int x,int y)
		{
			return (y * _width + x) * _bytesPerPixel;
		}

        protected Color GetPixel(int x, int y, byte[] rgbValues)
        {
            int idx = ToByteIdx(x,y);
            return new Color() { R = rgbValues[idx + _AddForR], G = rgbValues[idx + _AddForG], B = rgbValues[idx + _AddForB], A = (_AddForA < 0) ? (Byte) 255 : rgbValues[idx + _AddForA] };
        }

        protected void SetPixel(int x, int y, byte[] rgbValues, Color color)
        {
            int idx = ToByteIdx(x, y);
            rgbValues[idx + _AddForR] = color.R; 
            rgbValues[idx + _AddForG] = color.G; 
            rgbValues[idx + _AddForB] = color.B;

            if (_AddForA >= 0)
                rgbValues[idx + _AddForA] = color.A;
        }
        protected void SetPixel(int x, int y, byte[] rgbValues, int r, int g, int b, int a)
        {
            SetPixel(x, y, rgbValues, new Color() { R = Saturation(r), G = Saturation(g), B = Saturation(b), A = Saturation(a) });
        }
        protected void AddPixel(int x, int y, byte[] rgbValues, int r, int g, int b, int a)
        {
            Color pixel = GetPixel(x, y, rgbValues);
            SetPixel(x, y, rgbValues, pixel.R+r, pixel.G+g, pixel.B+b, pixel.A+a);
        }

        protected byte[] ReadImage(Bitmap imageX)
		{
			_height = imageX.Height;
			_width = imageX.Width;

			Rectangle rect = new Rectangle(0, 0, _width, _height);
			BitmapData bmpData = imageX.LockBits(rect, ImageLockMode.ReadOnly, imageX.PixelFormat);
			IntPtr ptr = bmpData.Scan0;
			int bytes = Math.Abs(bmpData.Stride) * _height;
			var rgbValues = new Byte[bytes];
			System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
			imageX.UnlockBits(bmpData);

            switch (imageX.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    _bytesPerPixel = 3;
                    _AddForA = -1;
                    break;
            }


            return rgbValues;
		}

        protected abstract void ConvertImage(byte[] rgbValues);

        protected Bitmap WriteImage(byte[] rgbValues,Bitmap imageX)
		{
			var bsrc = new Bitmap(_width, _height, imageX.PixelFormat);

			Rectangle rect = new Rectangle(0, 0, _width, _height);
			BitmapData bmpData = bsrc.LockBits(rect, ImageLockMode.WriteOnly, bsrc.PixelFormat);
			IntPtr ptr = bmpData.Scan0;

			int bytes = Math.Abs(bmpData.Stride) * _height;
			System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
			bsrc.UnlockBits(bmpData);

			var bdest = bsrc.Clone(rect, PixelFormat.Format1bppIndexed);
			bdest.SetResolution(imageX.HorizontalResolution, imageX.VerticalResolution);

			return bdest;
		}
	}

    #endregion
}
