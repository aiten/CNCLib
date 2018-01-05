////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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
using CNCLib.GCode;

namespace CNCLib.Wpf.Helpers
{
	public static class Extensions
	{
		public static string GetAxisName(this Logic.Contracts.DTO.Machine m, int axis)
		{
			return GCodeHelper.IndexToAxisName(axis);
		}

		public static decimal GetSize(this Logic.Contracts.DTO.Machine m, int axis)
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
		public static decimal GetProbeSize(this Logic.Contracts.DTO.Machine m, int axis)
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
