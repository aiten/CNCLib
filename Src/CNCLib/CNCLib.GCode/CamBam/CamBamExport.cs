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