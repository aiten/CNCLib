﻿////////////////////////////////////////////////////////
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
    class Line : Shape
    {
        public override Shape CreateShape()
        {
            return new Line();
        }

        public override string[] GetHPGLCommands()
        {
            string[] ret = new string[] 
            {
                HPGLHelper("PU",HPGLStart),
                HPGLHelper("PD",new Point(HPGLEnd.X,HPGLEnd.Y)),
            };
            return ret;
        }

        public override void Draw(PaintEventArgs e, PaintState paintstate)
        {
            MyDrawLine(e, GetForgroundPen(paintstate), DrawStart, DrawEnd);
        }
    }
}