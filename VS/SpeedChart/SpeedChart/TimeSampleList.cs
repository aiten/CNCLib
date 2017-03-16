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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedChart
{
	public class TimeSampleList : List<TimeSample>
	{
		public void ReadFiles(string filename)
		{
			Clear();

			using (StreamReader r = new StreamReader(filename))
			{
				string line;;

				while (!string.IsNullOrEmpty(line = r.ReadLine()))
				{
					Add(TimeSample.Parse(line));
				}
			}
		}

		public int DistFrom { get { return this.Min(x => x.Dist); } }
		public int DistTo { get { return this.Max(x => x.Dist); } }
		public int MaxSpeed { get { return this.Max(x => x.SysSpeed);} }
	}
}
