using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.Repository.Entities
{
	public class MachineCommand
	{
		public int MachineCommandID { get; set; }
		public string CommandString { get; set; }
		public int MachineID { get; set; }
		public virtual Machine Machine { get; set; }
	}
}
