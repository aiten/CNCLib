using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCode.Logic.Commands
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
