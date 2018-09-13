////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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

using Framework.Tools.Drawing;
using System;

namespace CNCLib.GCode.Commands
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
        void DrawLine(Command cmd, object param, DrawType drawtype, Point3D ptFrom, Point3D ptTo);

        void DrawArc(Command cmd,       object param, DrawType drawtype, Point3D ptFrom, Point3D ptTo, Point3D ptIIJ,
                     bool    clockwise, Pane   pane);

        void DrawEllipse(Command cmd, object param, DrawType drawtype, Point3D ptCenter, int xradius, int yradius);
    }
}