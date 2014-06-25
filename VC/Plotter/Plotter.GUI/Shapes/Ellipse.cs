////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

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
          Polygon(center,r,2*r*3/75,0,hpgl);
        }

        void Polygon(Point center, int radius, int n, int grad, List<string> hpgl)
        {
          // mittelpunkt!
          if (n>1)
          {
            int i;
            Point dp = new Point();
            Point pt = new Point(radius, 0);
			StringBuilder cmd = new StringBuilder();

            for (i=0;i<=n;i++)
            {
                double rad = ((360.0/n*i)+grad) / 180.0 * Math.PI;
                Rotate(pt, ref dp, rad);
                dp.Offset(center);
				if (i == 0) hpgl.Add(HPGLHelper("PU", dp));
				else if (cmd.Length == 0)
				{
					cmd.Append(HPGLHelper("PD", dp));
				}
				else
				{
					cmd.Append(HPGLHelper(",", dp));

					if (cmd.Length > 50)
					{
						hpgl.Add(cmd.ToString());
						cmd.Clear();
					}
				}
            }
			if (string.IsNullOrEmpty(cmd.ToString())==false)
			{
				hpgl.Add(cmd.ToString());
			}
          }
        }
    }
}
