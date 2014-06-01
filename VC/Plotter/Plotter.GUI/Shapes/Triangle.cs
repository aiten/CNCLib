using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Plotter.GUI.Shapes
{
    class Triangle : Shape
    {
        public override Shape CreateShape()
        {
            return new Triangle();
        }

        public override string[] GetHPGLCommands()
        {
            string[] ret = new string[] 
            {
                HPGLHelper("PU",HPGLStart),
                HPGLHelper("PD",new Point(HPGLStart.X,HPGLEnd.Y)),
                HPGLHelper("PD",new Point(HPGLEnd.X,HPGLStart.Y)),
                HPGLHelper("PD",new Point(HPGLStart.X,HPGLStart.Y)),
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
