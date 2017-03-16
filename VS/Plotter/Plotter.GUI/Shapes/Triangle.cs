////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
