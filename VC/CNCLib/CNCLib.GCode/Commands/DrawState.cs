using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.GCode.Commands
{
    public class DrawState
    {
        public bool UseLaser { get; set; } = false;
        public bool LaserOn { get; set; } = false;
        public bool SpindleOn { get; set; } = false;
        public bool CoolantOn { get; set; } = false;
    }
}
