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
