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

namespace SpeedChart
{
	public class TimeSample
	{
		public int Index { get; set; }
		public int TimerMS { get; set; }
		public int Dist { get; set; }
		public int SysSpeed { get; set; }
		public int XSpeed { get; set; }
		public int YSpeed { get; set; }
		public int ZSpeed { get; set; }
		public int ASpeed { get; set; }
		public int BSpeed { get; set; }
		public string Info { get; set; }

		public static TimeSample Parse(string line)
		{
			char[] delimiterChars = { ';' };
			string[] col = line.Split(delimiterChars, StringSplitOptions.None);

			TimeSample ts = new TimeSample();

			ts.Index = int.Parse(col[0]);
			ts.TimerMS = int.Parse(col[1]);
			ts.Dist = int.Parse(col[2]);
			ts.SysSpeed = int.Parse(col[3]);
			if (!string.IsNullOrEmpty(col[4])) ts.XSpeed = int.Parse(col[4]);
			if (!string.IsNullOrEmpty(col[5])) ts.YSpeed = int.Parse(col[5]);
			if (!string.IsNullOrEmpty(col[6])) ts.ZSpeed = int.Parse(col[6]);
			if (!string.IsNullOrEmpty(col[7])) ts.ASpeed = int.Parse(col[7]);
			if (!string.IsNullOrEmpty(col[8])) ts.BSpeed = int.Parse(col[8]);

			if (col.Length >= 20 && !string.IsNullOrEmpty(col[19])) ts.Info = col[19];

			return ts;
		}
	}
}
