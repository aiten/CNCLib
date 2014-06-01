using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Plotter.GUI.Shapes
{
    class Rectangle : Shape
    {
        public override Shape CreateShape()
        {
            return new Rectangle();
        }

        public override string[] GetHPGLCommands()
        {
            string[] ret = new string[] 
            {
                HPGLHelper("PU",HPGLStart),
                HPGLHelper("PD",new Point(HPGLStart.X,HPGLEnd.Y)),
                HPGLHelper("PD",new Point(HPGLEnd.X,HPGLEnd.Y)),
                HPGLHelper("PD",new Point(HPGLEnd.X,HPGLStart.Y)),
                HPGLHelper("PD",new Point(HPGLStart.X,HPGLStart.Y))
            };
            return ret;
        }

        public override void Draw(PaintEventArgs e, PaintState paintstate)
        {
            e.Graphics.DrawRectangle(GetForgroundPen(paintstate), NormalizedRect);
        }
    }
}
