using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Tools
{
	public class SpaceCoordinate
	{
		public SpaceCoordinate(decimal x, decimal y, decimal z)
		{
			X = x; Y = y; Z = z;
		}
		public SpaceCoordinate()
		{
			//X = new decimal?();
			//Y = new decimal?();
			//Z = new decimal?();
		}
		public decimal? X { get; set; }
		public decimal? Y { get; set; }
		public decimal? Z { get; set; }

		public bool HasAllValues { get { return X.HasValue && Y.HasValue && Z.HasValue;  } }

		public void AssignMissing(SpaceCoordinate from)
		{
			if (!X.HasValue && from.X.HasValue) X = from.X;
			if (!Y.HasValue && from.Y.HasValue) Y = from.Y;
			if (!Z.HasValue && from.Z.HasValue) Z = from.Z;
		}

		public static implicit operator System.Drawing.Point(SpaceCoordinate sc)
		{
			return new System.Drawing.Point((int)sc.X.Value, (int) sc.Y.Value);
		}

		public void Offset(SpaceCoordinate p)
		{
			if (X.HasValue && p.X.HasValue) X += p.X;
			if (Y.HasValue && p.Y.HasValue) Y += p.Y;
			if (Z.HasValue && p.Z.HasValue) Z += p.Z;
		}
	}
}
