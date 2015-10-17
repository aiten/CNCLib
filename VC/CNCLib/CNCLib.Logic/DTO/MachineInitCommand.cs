using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.Logic.DTO
{
	public class MachineInitCommand
	{
		public int MachineInitCommandID { get; set; }
		public int SeqNo { get; set; }
		public string CommandString { get; set; }
		public int MachineID { get; set; }
		public virtual Machine Machine { get; set; }
	}
}
