using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.Wpf.Helpers
{
	public static class Extensions
	{
		public static string GetAxisName(this CNCLib.Logic.Contracts.DTO.Machine m, int axis)
		{
			switch (axis)
			{
				case 0: return "X";
				case 1: return "Y";
				case 2: return "Z";
				case 3: return "A";
				case 4: return "B";
				case 5: return "C";
			}
			throw new NotImplementedException();
		}

		public static decimal GetSize(this CNCLib.Logic.Contracts.DTO.Machine m, int axis)
		{
			switch (axis)
			{
				case 0: return m.SizeX;
				case 1: return m.SizeY;
				case 2: return m.SizeZ;
				case 3: return m.SizeA;
				case 4: return m.SizeB;
				case 5: return m.SizeC;
			}
			throw new NotImplementedException();
		}
		public static decimal GetProbeSize(this CNCLib.Logic.Contracts.DTO.Machine m, int axis)
		{
			switch (axis)
			{
				case 0: return m.ProbeSizeX;
				case 1: return m.ProbeSizeY;
				case 2: return m.ProbeSizeZ;
			}
			return 0m;
		}
	}
}
