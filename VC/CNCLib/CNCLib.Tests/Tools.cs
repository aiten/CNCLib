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
using System.ComponentModel;

namespace CNCLib.Tests
{
	static class Tools
	{
		public static bool CompareProperties<T>(this T dest, T src)
		{
			foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(src))
			{
				if ((item.PropertyType == typeof(string) || item.PropertyType.IsValueType))
				{
					IComparable s = item.GetValue(src) as IComparable;
					IComparable d = item.GetValue(dest) as IComparable;

					if (s != null && d != null && s.CompareTo(d) != 0)
						return false;
				}
			}
			return true;
		}
	}
}
