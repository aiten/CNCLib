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

    public class Point3D
    {
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3D()
        {
            //X = new decimal?();
            //Y = new decimal?();
            //Z = new decimal?();
        }

        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }

        public double X0 => X ?? 0.0;
        public double Y0 => Y ?? 0.0;
        public double Z0 => Z ?? 0.0;

        public static implicit operator Point2D(Point3D pt)
        {
            return new Point2D { X = pt.X0, Y = pt.Y0 };
        }

        public double? this[int axis]
        {
            get
            {
                switch (axis)
                {
                    case 0:  return X;
                    case 1:  return Y;
                    case 2:  return Z;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (axis)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool Compare2D(Point3D to)
        {
            return Math.Abs(X0 - to.X0) < double.Epsilon && Math.Abs(Y0 - to.Y0) < double.Epsilon;
        }

        public bool HasAllValues => X.HasValue && Y.HasValue && Z.HasValue;

        public void AssignMissing(Point3D from)
        {
            if (!X.HasValue && from.X.HasValue)
            {
                X = @from.X;
            }

            if (!Y.HasValue && from.Y.HasValue)
            {
                Y = @from.Y;
            }

            if (!Z.HasValue && from.Z.HasValue)
            {
                Z = @from.Z;
            }
        }

        public static implicit operator System.Drawing.Point(Point3D sc)
        {
            return new System.Drawing.Point((int)(sc.X0), (int)(sc.Y0));
        }

        public void Offset(Point3D p)
        {
            if (X.HasValue && p.X.HasValue)
            {
                X += p.X;
            }

            if (Y.HasValue && p.Y.HasValue)
            {
                Y += p.Y;
            }

            if (Z.HasValue && p.Z.HasValue)
            {
                Z += p.Z;
            }
        }
    }
}