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

namespace CNCLib.GCode.Generate.Load;

using Framework.Drawing;

public partial class LoadHpgl
{
    class HpglCommand
    {
        public enum HpglCommandType
        {
            PenUp,
            PenDown,
            Other
        }

        public HpglCommandType CommandType { get; set; } = HpglCommandType.Other;

        public bool IsPenCommand =>
            CommandType == HpglCommandType.PenUp || CommandType == HpglCommandType.PenDown;

        public bool     IsPenDownCommand      => CommandType == HpglCommandType.PenDown;
        public bool     IsPointToValid        => IsPenCommand;
        public Point3D? PointFrom             { get; set; }
        public Point3D  PointTo               { get; set; } = default!;
        public double?  LineAngle             { get; set; }
        public double?  DiffLineAngleWithNext { get; set; }
        public string   CommandString         { get; set; } = default!;

        public void ResetCalculated()
        {
            PointFrom             = null;
            DiffLineAngleWithNext = null;
            LineAngle             = null;
        }
    }
}