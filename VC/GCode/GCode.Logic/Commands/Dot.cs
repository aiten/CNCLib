using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace GCode.GUI.Shapes
{
    class Dot : Command
    {
        public override Command CreateCommand()
        {
            return new Dot();
        }

        public override string[] GetMm1000Commands()
        {
            string[] ret = new string[] 
            {
                GCodeHelper("PU",Mm1000Start)
            };
            return ret;
        }

        public override void Draw(PaintEventArgs e, PaintState paintstate)
        {
            MyDrawLine(e, GetForgroundPen(paintstate), DrawStart, DrawStart);
        }
    }
}
