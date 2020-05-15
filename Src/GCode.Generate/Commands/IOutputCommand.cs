/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

using System;

using Framework.Drawing;

namespace CNCLib.GCode.Generate.Commands
{
    [Flags]
    public enum DrawType
    {
        NoDraw   = 0,
        Draw     = 1,
        Cut      = 2, // Go or G1,G2,G3
        Laser    = 4,
        Selected = 8, // e.g. current (focused or in work) commands
        Done     = 16
    }

    public enum Pane
    {
        XYPane,
        XZPane,
        YZPane
    }

    public interface IOutputCommand
    {
        void DrawLine(Command cmd, object param, DrawType drawType, Point3D ptFrom, Point3D ptTo);

        void DrawArc(Command cmd, object param, DrawType drawType, Point3D ptFrom, Point3D ptTo, Point3D ptIJK, bool clockwise, Pane pane);

        void DrawEllipse(Command cmd, object param, DrawType drawType, Point3D ptCenter, int radiusX, int radiusY);
    }
}