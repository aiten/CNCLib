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
        public decimal SizeX { get; set; }
        public decimal SizeY { get; set; }
        public decimal SizeZ { get; set; }
    }
}
