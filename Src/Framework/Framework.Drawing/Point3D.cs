////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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