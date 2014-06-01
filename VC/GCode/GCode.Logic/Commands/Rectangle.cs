using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GCode.GUI.Shapes
{
    class Rectangle : Command
    {
        public override Command CreateCommand()
        {
            return new Rectangle();
        }

        public override string[] GetMm1000Commands()
        {
            string[] ret = new string[] 
            {
                GCodeHelper("PU",Mm1000Start),
                GCodeHelper("PD",new Point(Mm1000Start.X,EndPosition.Y)),
                GCodeHelper("PD",new Point(EndPosition.X,EndPosition.Y)),
                GCodeHelper("PD",new Point(EndPosition.X,Mm1000Start.Y)),
                GCodeHelper("PD",new Point(Mm1000Start.X,Mm1000Start.Y))
            };
            return ret;
        }

        public override void Draw(PaintEventArgs e, PaintState paintstate)
        {
            e.Graphics.DrawRectangle(GetForgroundPen(paintstate), NormalizedRect);
        }
    }
}
