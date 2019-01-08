////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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

using Framework.Drawing;

namespace CNCLib.GCode.Load
{
    public partial class LoadHPGL
    {
        class HPGLCommand
        {
            public enum HPGLCommandType
            {
                PenUp,
                PenDown,
                Other
            }

            public HPGLCommandType CommandType { get; set; } = HPGLCommandType.Other;

            public bool IsPenCommand =>
                CommandType == HPGLCommandType.PenUp || CommandType == HPGLCommandType.PenDown;

            public bool    IsPenDownCommand      => CommandType == HPGLCommandType.PenDown;
            public bool    IsPointToValid        => IsPenCommand;
            public Point3D PointFrom             { get; set; }
            public Point3D PointTo               { get; set; }
            public double? LineAngle             { get; set; }
            public double? DiffLineAngleWithNext { get; set; }
            public string  CommandString         { get; set; }

            public void ResetCalculated()
            {
                PointFrom             = null;
                DiffLineAngleWithNext = null;
                LineAngle             = null;
            }
        }
    }
}