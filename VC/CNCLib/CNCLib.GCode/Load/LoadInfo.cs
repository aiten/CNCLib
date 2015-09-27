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

namespace CNCLib.GCode.Load
{
	public class LoadInfo
	{
		public LoadInfo() { SwapXY = false; ScaleX = 1; ScaleY = 1; OfsX = 0; OfsY = 0; AutoScale = true; AutoScaleKeepRatio = true; AutoScaleBorderDistX = 0.5m; AutoScaleBorderDistY = 0.5m; }

		public String FileName { get; set; }
		public bool SwapXY { get; set; }
		public decimal ScaleX { get; set; }
		public decimal ScaleY { get; set; }
		public decimal OfsX { get; set; }
		public decimal OfsY { get; set; }

		public bool AutoScale { get; set; }
		public bool AutoScaleKeepRatio { get; set; }

		public decimal AutoScaleSizeX { get; set; }
		public decimal AutoScaleSizeY { get; set; }

		public decimal AutoScaleBorderDistX { get; set; }
		public decimal AutoScaleBorderDistY { get; set; }
	}
}
