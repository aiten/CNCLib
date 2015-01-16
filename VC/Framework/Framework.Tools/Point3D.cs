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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Tools
{
	public class Point3D
	{
		public Point3D(decimal x, decimal y, decimal z)
		{
			X = x; Y = y; Z = z;
		}
		public Point3D()
		{
			//X = new decimal?();
			//Y = new decimal?();
			//Z = new decimal?();
		}
		public decimal? X { get; set; }
		public decimal? Y { get; set; }
		public decimal? Z { get; set; }

		public bool HasAllValues { get { return X.HasValue && Y.HasValue && Z.HasValue;  } }

		public void AssignMissing(Point3D from)
		{
			if (!X.HasValue && from.X.HasValue) X = from.X;
			if (!Y.HasValue && from.Y.HasValue) Y = from.Y;
			if (!Z.HasValue && from.Z.HasValue) Z = from.Z;
		}

		public static implicit operator System.Drawing.Point(Point3D sc)
		{
			return new System.Drawing.Point((int)sc.X.Value, (int) sc.Y.Value);
		}

		public void Offset(Point3D p)
		{
			if (X.HasValue && p.X.HasValue) X += p.X;
			if (Y.HasValue && p.Y.HasValue) Y += p.Y;
			if (Z.HasValue && p.Z.HasValue) Z += p.Z;
		}
	}
}
