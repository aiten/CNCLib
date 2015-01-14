using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxxon.Logic.DTO
{
	public class Machine
	{
		public int MachineID { get; set; }
		public string ComPort { get; set; }
		public int BaudRate { get; set; }
		public string Name { get; set; }
		public decimal SizeX { get; set; }
		public decimal SizeY { get; set; }
		public decimal SizeZ { get; set; }
		public int BufferSize { get; set; }
		public bool CommandToUpper { get; set; }
        public bool Default { get; set; }
	}
}
