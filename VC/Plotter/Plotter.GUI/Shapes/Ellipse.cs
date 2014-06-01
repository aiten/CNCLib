using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Plotter.GUI.Shapes
{
    class Ellipse : Shape
    {
        public override Shape CreateShape()
        {
            return new Ellipse();
        }

        public override string[] GetHPGLCommands()
        {
            System.Drawing.Rectangle rec = HPGLNormalizedRect;
            List<string> hpgl = new List<string>();

            Point center = new Point(rec.X+rec.Width/2,rec.Y+rec.Height/2);

            Circle(center, rec.Width/2, hpgl);

            return hpgl.ToArray();
        }

        public override void Draw(PaintEventArgs e, PaintState paintstate)
        {
            e.Graphics.DrawEllipse(GetForgroundPen(paintstate), NormalizedRect);
        }

        void Rotate(Point p, ref Point dp, double rad)
        {
	        double mysin = Math.Sin(rad);
            double mycos = Math.Cos(rad);

            dp.X = (int)((double)p.X * mycos + (double)p.Y * mysin);
            dp.Y = (int)((double)p.Y * mycos - (double)p.X * mysin);
        }

        void Circle(Point center, int r, List<string> hpgl)
        {
          Polygon(center,r,2*r*3/50,0,hpgl);
        }

        void Polygon(Point center, int radius, int n, int grad, List<string> hpgl)
        {
          // mittelpunkt!
          if (n>1)
          {
            int i;
            Point dp = new Point();
            Point pt = new Point(radius, 0);

            for (i=0;i<=n;i++)
            {
                double rad = ((360.0/n*i)+grad) / 180.0 * Math.PI;
                Rotate(pt, ref dp, rad);
                dp.Offset(center);
                hpgl.Add(HPGLHelper(i == 0 ? "PU" : "PD", dp));
            }
          }
        }
    }
}
