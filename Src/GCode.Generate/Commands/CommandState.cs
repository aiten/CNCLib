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

namespace CNCLib.GCode.Generate.Commands
{
    using System.Collections.Generic;

    public class CommandState
    {
        public bool UseLaser    { get; set; } = false;
        public bool LaserOn     { get; set; } = false;
        public bool SpindleOn   { get; set; } = false;
        public bool CoolantOn   { get; set; } = false;
        public Pane CurrentPane { get; set; } = Pane.XYPane;

        public bool IsSelected { get; set; } = true;

        public Command.Variable G82R { get; set; }
        public Command.Variable G82P { get; set; }
        public Command.Variable G82Z { get; set; }

        public Dictionary<int, double> ParameterValues { get; private set; } = new Dictionary<int, double>();
    }
}