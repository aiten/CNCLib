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


using System.Collections.Generic;
using Framework.Tools.Drawing;

namespace CNCLib.GCode.Commands
{
	[IsGCommand]
	class G82Command : DrillCommand
    {
		#region crt + factory

		public G82Command()
		{
			Code = "G82";
		}

		#endregion

		#region GCode
		public override string[] GetGCodeCommands(Point3D startfrom, CommandState state) 
		{
			string[] ret;
			if (Settings.Instance.SubstDrillCycle)
			{
				// from
				// G82 X-8.8900 Y3.8100  Z-0.2794 F400 R0.5000  P0.000000
				// next command with R and P G82 X-11.4300 Y3.8100
				// to
				// G00 X-8.8900 Y3.8100
				// G01 Z-0.2794 F400
				// (G04 P0)
				// G00 Z0.5000

				double r;
				if (TryGetVariable('R',out r))
				{
					state.G82_R = r;
				}
				else
				{
					r = state.G82_R ?? 0;
				}
				double p;
				if (TryGetVariable('P', out p))
				{
					state.G82_P = p;
				}
				else
				{
					p = state.G82_P ?? 0;
				}
				double z;
				if (TryGetVariable('Z', out z))
				{
					state.G82_Z = z;
				}
				else
				{
					z = state.G82_Z ?? 0;
				}

				var move1 = new G00Command();
				CopyVariable('X', move1);
				CopyVariable('Y', move1);

				var move2 = new G01Command();
				move2.AddVariable('Z', z);
				CopyVariable('F', move2);

				var move3 = new G04Command();
				move3.AddVariable('P', p);

				var move4 = new G00Command();
				move4.AddVariable('Z', r);

				var list = new List<string>();
				list.AddRange(move1.GetGCodeCommands(startfrom, state));
				list.AddRange(move2.GetGCodeCommands(startfrom, state));
				if (p != 0.0)
					list.AddRange(move3.GetGCodeCommands(startfrom, state));
				list.AddRange(move4.GetGCodeCommands(startfrom, state));

				ret = list.ToArray();
			}
			else
			{
				ret = base.GetGCodeCommands(startfrom,state);
			}

			return ret;
		}

		#endregion

		#region Draw

		#endregion
	}
}
