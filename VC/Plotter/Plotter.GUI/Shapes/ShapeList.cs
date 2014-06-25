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
using System.Threading.Tasks;

namespace Plotter.GUI.Shapes
{
	public class ShapeList : List<Shape>
	{
		public struct PaintState
		{
			public int CommandIdx;
			public int SelectedIdx;
		}

		public void AddShape(Shape cmd)
		{
			if (Count > 0)
			{
				this[Count-1].NextShape = cmd;
				cmd.PrevShape = this[Count - 1];
			}
			cmd.NextShape = null;
			base.Add(cmd);
		}

		public new void Add(Shape cmd) 
		{
			AddShape(cmd); 
		}

/*
		public void UpdateCache()
		{
			foreach (Shape cmd in this)
			{
				cmd.UpdateCalculatedEndPosition();
			}
		}
*/ 
/*
		public void Paint(IOutputCommand output, object param)
		{
			foreach (Command cmd in this)
			{
				cmd.Draw(output,param);
			}
		}
 */
	}
}
