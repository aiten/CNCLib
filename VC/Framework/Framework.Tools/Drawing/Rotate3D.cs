////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
using Framework.Tools.Drawing;

namespace Framework.Tools.Drawing
{
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

		double[,] _vect = new double[3, 3];

		public void Set3DRotate(double rad, double[] vect)
		{
			double n1 = (double)vect[0];
			double n2 = (double)-vect[1];
			double n3 = (double)vect[2];

			double vectorlenght = Math.Sqrt(n1 * n1 + n2 * n2 + n3 * n3);
			n1 = n1 / vectorlenght;
			n2 = n2 / vectorlenght;
			n3 = n3 / vectorlenght;

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

			return new Point3D(
				fx * _vect[0, 0] + fy * _vect[0, 1] + fz * _vect[0, 2],
				fx * _vect[1, 0] + fy * _vect[1, 1] + fz * _vect[1, 2],
				fx * _vect[2, 0] + fy * _vect[2, 1] + fz * _vect[2, 2]
				);
		}

		public Point3D Rotate(Point3D pt)
		{
			double x = pt.X ?? 0;
			double y = pt.Y ?? 0;
			double z = pt.Z ?? 0;

			Rotate(ref x, ref y, ref z);

			return new Point3D(x, y, z);
		}
	}


	#endregion
}
