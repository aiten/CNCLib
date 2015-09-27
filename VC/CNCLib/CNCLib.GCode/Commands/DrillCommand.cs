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

namespace CNCLib.GCode.Commands
{
    class DrillCommand : Command
    {
		#region crt + factory

		public DrillCommand()
		{
			PositionValid = true;
			Movetype = MoveType.Fast;
		}

		#endregion

		#region GCode

		#endregion

		#region Draw

		public override void Draw(IOutputCommand output, object param)
		{
			base.Draw(output, param);
			output.DrawEllipse(this, param, MoveType.Normal, CalculatedEndPosition, 10, 10);
        }

		#endregion
	}
}
