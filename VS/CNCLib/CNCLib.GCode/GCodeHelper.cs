////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
