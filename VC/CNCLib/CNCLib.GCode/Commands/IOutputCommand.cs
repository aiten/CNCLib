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

using Framework.Tools;

namespace CNCLib.GCode.Commands
{
	public interface IOutputCommand
	{
		void DrawLine(Command cmd, object param, Command.MoveType movetype, Point3D ptFrom, Point3D ptTo);
		void DrawEllipse(Command cmd, object param, Command.MoveType movetype, Point3D ptFrom, int xradius, int yradius);
	}
}
