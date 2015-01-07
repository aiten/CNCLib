////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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
using System.Drawing;
using Framework.Tools;

namespace Proxxon.GCode.Commands
{
	[IsGCommand("G3,G03")]
	class G03Command : Command
    {
		#region crt + factory

		public G03Command()
		{
			PositionValid = true;
			Movetype = MoveType.Normal;
			Code = "G3";
		}

		#endregion

		#region GCode

		#endregion

		#region Draw

		public override void Draw(IOutputCommand output, object param)
		{
			base.Draw(output, param);
			/*

						Rectangle rect;
						Point start = DrawStartPosition;
						rect = new Rectangle(start, new Size(DrawEndPosition.X - start.X, DrawEndPosition.Y - start.Y));
						e.Graphics.DrawEllipse(GetForgroundPen(paintstate), rect);
			 */
		}

		#endregion
	}
}
