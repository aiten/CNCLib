using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.GCode
{
	public class GCodeHelper
	{
		static public int AxisNameToIndex(string axisname)
		{
			if (axisname.Length == 1)
			{
				return AxisNameToIndex(axisname[0]);
			}
			return -1;
		}
		static public int AxisNameToIndex(char axisname)
		{
			switch (char.ToUpper(axisname))
			{
				case 'X': return 0;
				case 'Y': return 1;
				case 'Z': return 2;
				case 'A': return 3;
				case 'B': return 4;
				case 'C': return 5;
			}
			return -1;
		}

		static public string IndexToAxisName(int axis)
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
			throw new ArgumentOutOfRangeException();
		}
	}
}
