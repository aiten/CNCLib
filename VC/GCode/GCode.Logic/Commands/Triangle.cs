using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace GCode.GUI.Shapes
{
    class Triangle : Command
    {
        public override Command CreateCommand()
        {
            return new Triangle();
        }

        public override string[] GetMm1000Commands()
        {
            string[] ret = new string[] 
            {
                GCodeHelper("PU",Mm1000Start),
                GCodeHelper("PD",new Point(Mm1000Start.X,EndPosition.Y)),
                GCodeHelper("PD",new Point(EndPosition.X,Mm1000Start.Y)),
                GCodeHelper("PD",new Point(Mm1000Start.X,Mm1000Start.Y)),
            };
            return ret;
        }

        public override void Draw(PaintEventArgs e, PaintState paintstate)
        {
            Point[] pts = new Point[3];
            pts[0] = DrawStart;
            pts[1] = new Point(DrawStart.X, DrawEnd.Y);
            pts[2] = new Point(DrawEnd.X, DrawStart.Y);
            e.Graphics.DrawPolygon(GetForgroundPen(paintstate), pts);
        }
    }
}
