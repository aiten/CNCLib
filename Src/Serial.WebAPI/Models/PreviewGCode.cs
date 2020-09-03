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

namespace CNCLib.Serial.WebAPI.Models
{
    using System.Collections.Generic;

    public class PreviewGCode
    {
        public double SizeX { get; set; }

        public double SizeY { get; set; }

        public double SizeZ { get; set; }

        public bool KeepRatio { get; set; }

        public double Zoom { get; set; }

        public double OffsetX { get; set; }

        public double OffsetY { get; set; }

        public double OffsetZ { get; set; }

        public double CutterSize { get; set; }

        public double LaserSize { get; set; }

        public string MachineColor { get; set; }

        public string LaserOnColor { get; set; }

        public string LaserOffColor { get; set; }

        public string CutColor { get; set; }

        public string CutDotColor { get; set; }

        public string CutEllipseColor { get; set; }

        public string CutArcColor { get; set; }

        public string FastMoveColor { get; set; }

        public string HelpLineColor { get; set; }

        // public Rotate3D Rotate { get; set; }

        public double Rotate3DAngle { get; set; }

        public IEnumerable<double> Rotate3DVect { get; set; }

        public int RenderSizeX { get; set; }

        public int RenderSizeY { get; set; }
    }
}