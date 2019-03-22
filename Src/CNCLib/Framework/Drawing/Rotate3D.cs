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
    using System;

    public class Rotate3D
    {
        public Rotate3D(double rad, double[] vect)
        {
            Set3DRotate(rad, vect);
        }

        public Rotate3D()
        {
            Set3DRotate(0, new double[] { 0, 0, 1 });
        }

        #region 3D Rotate

        readonly double[,] _vect = new double[3, 3];

        public void Set3DRotate(double rad, double[] vect)
        {
            double n1 = vect[0];
            double n2 = -vect[1];
            double n3 = vect[2];

            double vectorLength = Math.Sqrt(n1 * n1 + n2 * n2 + n3 * n3);
            n1 = n1 / vectorLength;
            n2 = n2 / vectorLength;
            n3 = n3 / vectorLength;

            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);

            _vect[0, 0] = n1 * n1 * (1 - cos) + cos;
            _vect[0, 1] = n1 * n2 * (1 - cos) - n3 * sin;
            _vect[0, 2] = n1 * n3 * (1 - cos) + n2 * sin;

            _vect[1, 0] = n1 * n2 * (1 - cos) + n3 * sin;
            _vect[1, 1] = n2 * n2 * (1 - cos) + cos;
            _vect[1, 2] = n2 * n3 * (1 - cos) - n1 * sin;

            _vect[2, 0] = n1 * n3 * (1 - cos) - n2 * sin;
            _vect[2, 1] = n2 * n3 * (1 - cos) + n1 * sin;
            _vect[2, 2] = n3 * n3 * (1 - cos) + cos;
        }

        /////////////////////////////////////////////////////////

        public void Rotate(ref double x, ref double y, ref double z)
        {
            double fx = x;
            double fy = y;
            double fz = z;

            x = fx * _vect[0, 0] + fy * _vect[0, 1] + fz * _vect[0, 2];
            y = fx * _vect[1, 0] + fy * _vect[1, 1] + fz * _vect[1, 2];
            z = fx * _vect[2, 0] + fy * _vect[2, 1] + fz * _vect[2, 2];
        }

        public Point3D Rotate(double x, double y, double z)
        {
            double fx = x;
            double fy = y;
            double fz = z;

            return new Point3D(fx * _vect[0, 0] + fy * _vect[0, 1] + fz * _vect[0, 2], fx * _vect[1, 0] + fy * _vect[1, 1] + fz * _vect[1, 2], fx * _vect[2, 0] + fy * _vect[2, 1] + fz * _vect[2, 2]);
        }

        public Point3D Rotate(Point3D pt)
        {
            double x = pt.X0;
            double y = pt.Y0;
            double z = pt.Z0;

            Rotate(ref x, ref y, ref z);

            return new Point3D(x, y, z);
        }
    }

    #endregion
}