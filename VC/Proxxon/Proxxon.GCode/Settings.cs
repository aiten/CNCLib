using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Tools;

namespace Proxxon.GCode
{
    public class Settings : Singleton<Settings>
    {
        public Settings()
        {
            SizeX = 130.0m;
            SizeY = 45.0m;
            SizeZ = 81.0m;
        }

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
		public decimal ProbeSizeX { get; set; }
		public decimal ProbeSizeY { get; set; }
		public decimal ProbeSizeZ { get; set; }

    }
}
