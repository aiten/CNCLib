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

namespace CNCLib.GCode.Commands
{
	public class CommandList :	List<Command>
	{
		public struct PaintState
		{
			public int CommandIdx;
			public int SelectedIdx;
		}

		public void AddCommand(Command cmd)
		{
			if (Count > 0)
			{
				this[Count-1].NextCommand = cmd;
				cmd.PrevCommand = this[Count - 1];
			}
			cmd.NextCommand = null;
			base.Add(cmd);
		}

		public new void Add(Command cmd) 
		{ 
			AddCommand(cmd); 
		}


		public void UpdateCache()
		{
			foreach (Command cmd in this)
			{
				cmd.UpdateCalculatedEndPosition();
			}
		}
		public void Paint(IOutputCommand output, object param)
		{
			foreach (Command cmd in this)
			{
				cmd.Draw(output,param);
			}
		}
	}
}
