using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Plotter.GUI.Shapes
{
    class Dot : Shape
    {
        public override Shape CreateShape()
        {
            return new Dot();
        }

        public override string[] GetHPGLCommands()
        {
            string[] ret = new string[] 
            {
                HPGLHelper("PU",HPGLStart)
            };
            return ret;
        }

        public override void Draw(PaintEventArgs e, PaintState paintstate)
        {
            MyDrawLine(e, GetForgroundPen(paintstate), DrawStart, DrawStart);
        }
    }
}
