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

using Framework.Tools.Pattern;

namespace CNCLib.GCode
{
	public class Settings : Singleton<Settings>
    {
        public Settings()
        {
            SizeX = 130.0m;
            SizeY = 45.0m;
            SizeZ = 81.0m;
			ResetOnConnect = false;
		}

		public decimal SizeX { get; set; }
		public decimal SizeY { get; set; }
		public decimal SizeZ { get; set; }


		public bool ResetOnConnect { get; set; }

	}
}
