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
using System.Drawing;
using System.Windows.Forms;

namespace Plotter.GUI.Shapes
{
    public abstract class Shape
    {
        public enum Shapetype
        {
            Ellipse,
            Line,
            Rectangle,
            Triangle
        };

        public struct PaintState
        {
            public int ShapeIdx;
            public int SelectedIdx;
        }

        public delegate Point HPGLToClient(Point pt);

        public abstract Shape CreateShape();

		public Shape NextShape { get; set; }
		public Shape PrevShape { get; set; }

        public Point DrawStart { get; set; }
        public Point DrawEnd { get; set; }
        public Color ForgroundColor { get; set; }
        public int LineSize { get; set; }

        public Point HPGLStart { get; set; }
        public Point HPGLEnd { get; set; }

        public Pen GetForgroundPen(PaintState paintstate) 
        {
            if (paintstate.SelectedIdx == paintstate.ShapeIdx)
                return new Pen(Color.Red,5);
            return new Pen(ForgroundColor, paintstate.SelectedIdx > paintstate.ShapeIdx ? 2 : 1);
        }

        public System.Drawing.Rectangle NormalizedRect
        {
            get
            {
                return new System.Drawing.Rectangle(
                                Math.Min(DrawStart.X, DrawEnd.X),
                                Math.Min(DrawStart.Y, DrawEnd.Y),
                                Math.Abs(DrawStart.X - DrawEnd.X),
                                Math.Abs(DrawStart.Y - DrawEnd.Y)
                                );
            }
        }
        public System.Drawing.Rectangle HPGLNormalizedRect
        {
            get
            {
                return new System.Drawing.Rectangle(
                                Math.Min(HPGLStart.X, HPGLEnd.X),
                                Math.Min(HPGLStart.Y, HPGLEnd.Y),
                                Math.Abs(HPGLStart.X - HPGLEnd.X),
                                Math.Abs(HPGLStart.Y - HPGLEnd.Y)
                                );
            }
        }

        protected void MyDrawLine(PaintEventArgs e, Pen pen, Point ptFrom, Point ptTo)
        {
            if (ptFrom.Equals(ptTo))
            {
                e.Graphics.DrawEllipse(pen, ptFrom.X, ptFrom.Y, 1, 1);
            }
            else
            {
                e.Graphics.DrawLine(pen, ptFrom, ptTo);
            }
        }

        protected static string HPGLHelper(string cmd, Point pt)
        {
//            int z = cmd == "PD" ? 80000 : 96000;
//            return "z " + z.ToString() + ";a " + pt.X * 20 + " " + pt.Y * 12 + " " + z.ToString(); ;
            return cmd + " " + pt.X + "," + pt.Y;
        }

        public Shape()
        {
            ForgroundColor = Color.Black;
            LineSize = 1;
        }

        public virtual void AdjustDrawPos(HPGLToClient toDrawPos)
        {
            DrawStart = toDrawPos(HPGLStart);
            DrawEnd = toDrawPos(HPGLEnd);
        }

        public abstract string[] GetHPGLCommands();

        public abstract void Draw(PaintEventArgs e, PaintState paintstate);
    }
}
