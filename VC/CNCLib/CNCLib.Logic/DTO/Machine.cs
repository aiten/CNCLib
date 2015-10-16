using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.Logic.DTO
{
	public class Machine
	{
		public int MachineID { get; set; }
		public string ComPort { get; set; }
		public int BaudRate { get; set; }
		public int Axis { get; set; }
		public string Name { get; set; }
		public decimal SizeX { get; set; }
		public decimal SizeY { get; set; }
		public decimal SizeZ { get; set; }
		public decimal SizeA { get; set; }
		public decimal SizeB { get; set; }
		public decimal SizeC { get; set; }
		public int BufferSize { get; set; }
		public bool CommandToUpper { get; set; }
		public decimal ProbeSizeX { get; set; }
		public decimal ProbeSizeY { get; set; }
		public decimal ProbeSizeZ { get; set; }
		public decimal ProbeDistUp { get; set; }
		public decimal ProbeDist { get; set; }
		public decimal ProbeFeed { get; set; }
		public bool SDSupport { get; set; }
		public bool Spindle { get; set; }
		public bool Coolant { get; set; }
		public bool Rotate { get; set; }

		public List<MachineCommand> MachineCommands { get; set; }

	}
}
