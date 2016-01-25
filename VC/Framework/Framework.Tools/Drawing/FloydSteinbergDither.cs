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
    public class FloydSteinbergDither
	{
        #region private members

        int _bytesPerPixel = 4;
		int _height;
		int _width;

        int _AddForA = 3;
        int _AddForR = 2;
        int _AddForG = 1;
        int _AddForB = 0;

        #endregion

        #region properties

        public int Graythreshold { get; set; } = 127;

        #endregion

        #region public

        public Bitmap Process(Bitmap imageX)
        {
            byte[] rgbValues = ReadImage(imageX);

            ConvertImage(rgbValues);

            return WriteImage(rgbValues,imageX);
        }

        #endregion

        #region private helper

        private static byte Saturation(int r)
		{
			if (r <= 0)
				return 0;
			else if (r > 255)
				return (byte)255;
			else
				return (byte)(r);
		}

		private bool FindNearestColorGrayScale(Byte colorR, Byte colorG, Byte colorB)
		{
			return (0.2126 * colorR + 0.7152 * colorG + 0.0722 * colorB) >= Graythreshold;
		}

		private int ToByteIdx(int x,int y)
		{
			return (y * _width + x) * _bytesPerPixel;
		}

		private byte[] ReadImage(Bitmap imageX)
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

		private void ConvertImage(byte[] rgbValues)
		{
			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					int idx = ToByteIdx(x, y);
					Byte currentPixelR = rgbValues[idx + _AddForR];
					Byte currentPixelG = rgbValues[idx + _AddForG];
					Byte currentPixelB = rgbValues[idx + _AddForB];

					Byte bestColorRGB = (Byte) (FindNearestColorGrayScale(currentPixelR, currentPixelG, currentPixelB) ? 255 : 0);

                    if (_AddForA >= 0)
					    rgbValues[idx + _AddForA] = 255;
					rgbValues[idx + _AddForR] = bestColorRGB;
					rgbValues[idx + _AddForG] = bestColorRGB;
					rgbValues[idx + _AddForB] = bestColorRGB;

					int errorR = (currentPixelR) - (bestColorRGB);
					int errorG = (currentPixelG) - (bestColorRGB);
					int errorB = (currentPixelB) - (bestColorRGB);
					if (x + 1 < _width)
					{
						idx = ToByteIdx(x + 1, y + 0);
						rgbValues[idx + _AddForR] = Saturation(rgbValues[idx + _AddForR] + ((errorR * 7) >> 4));
						rgbValues[idx + _AddForG] = Saturation(rgbValues[idx + _AddForG] + ((errorG * 7) >> 4));
						rgbValues[idx + _AddForB] = Saturation(rgbValues[idx + _AddForB] + ((errorB * 7) >> 4));
					}
					if (y + 1 < _height)
					{
						if (x - 1 > 0)
						{
							idx = ToByteIdx(x - 1, y + 1);
							rgbValues[idx + _AddForR] = Saturation(rgbValues[idx + _AddForR] + ((errorR * 3) >> 4));
							rgbValues[idx + _AddForG] = Saturation(rgbValues[idx + _AddForG] + ((errorG * 3) >> 4));
							rgbValues[idx + _AddForB] = Saturation(rgbValues[idx + _AddForB] + ((errorB * 3) >> 4));
						}
						idx = ToByteIdx(x + 0, y + 1);
						rgbValues[idx + _AddForR] = Saturation(rgbValues[idx + _AddForR] + ((errorR * 5) >> 4));
						rgbValues[idx + _AddForG] = Saturation(rgbValues[idx + _AddForG] + ((errorG * 5) >> 4));
						rgbValues[idx + _AddForB] = Saturation(rgbValues[idx + _AddForB] + ((errorB * 5) >> 4));
						if (x + 1 < _width)
						{
							idx = ToByteIdx(x + 1, y + 1);
							rgbValues[idx + _AddForR] = Saturation(rgbValues[idx + _AddForR] + ((errorR * 1) >> 4));
							rgbValues[idx + _AddForG] = Saturation(rgbValues[idx + _AddForG] + ((errorG * 1) >> 4));
							rgbValues[idx + _AddForB] = Saturation(rgbValues[idx + _AddForB] + ((errorB * 1) >> 4));
						}
					}
				}
			}
		}

		private Bitmap WriteImage(byte[] rgbValues,Bitmap imageX)
		{
			Byte[] Bits = new Byte[_width * _height * _bytesPerPixel];
			GCHandle BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
			var b = new Bitmap(_width, _height, _width * _bytesPerPixel, imageX.PixelFormat, BitsHandle.AddrOfPinnedObject());


            Buffer.BlockCopy(rgbValues, 0, Bits, 0, rgbValues.Length);

			Bitmap result = new Bitmap(b);
			BitsHandle.Free();

			var rectangle = new Rectangle(0, 0, _width, _height);

            var b2 = result.Clone(rectangle, PixelFormat.Format1bppIndexed);
            b2.SetResolution(imageX.HorizontalResolution, imageX.VerticalResolution);

            return b2;
		}
	}

    #endregion
}
