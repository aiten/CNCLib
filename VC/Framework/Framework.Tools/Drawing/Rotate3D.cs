using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			double cosa = Math.Cos(rad);
			double sina = Math.Sin(rad);

			_vect[0, 0] = n1 * n1 * (1 - cosa) + cosa;
			_vect[0, 1] = n1 * n2 * (1 - cosa) - n3 * sina;
			_vect[0, 2] = n1 * n3 * (1 - cosa) + n2 * sina;

			_vect[1, 0] = n1 * n2 * (1 - cosa) + n3 * sina;
			_vect[1, 1] = n2 * n2 * (1 - cosa) + cosa;
			_vect[1, 2] = n2 * n3 * (1 - cosa) - n1 * sina;

			_vect[2, 0] = n1 * n3 * (1 - cosa) - n2 * sina;
			_vect[2, 1] = n2 * n3 * (1 - cosa) + n1 * sina;
			_vect[2, 2] = n3 * n3 * (1 - cosa) + cosa;
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

		public Point3D Rotate(Point3D pt)
		{
			double x = (double) (pt.X ?? 0);
			double y = (double) (pt.Y ?? 0);
			double z = (double) (pt.Z ?? 0);

			Rotate(ref x, ref y, ref z);

			return new Point3D((decimal)x, (decimal)y, (decimal)z);
		}
	}


	#endregion
}
