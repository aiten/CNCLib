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

using System.Collections.Generic;
using System.Xml.Serialization;

using Framework.Drawing;

namespace CNCLib.GCode.CamBam
{
    public class CamBamExport
    {
        [XmlType("layer")]
        public class Layer
        {
            public string Name { get; set; }

            public System.Drawing.Color Color { get; set; }

            public List<Object> Objects { get; set; } = new List<Object>();
        }

        public List<Layer> Lasers { get; set; } = new List<Layer>();

        public class Object
        {
            public int Id { get; set; }
        }

        public class PLine : Object
        {
            public bool Closed { get; set; }

            public List<Point3D> Pts { get; set; }
        }
    }
}