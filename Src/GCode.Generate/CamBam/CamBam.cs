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

namespace CNCLib.GCode.Generate.CamBam;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

[XmlRoot("CADFile", IsNullable = false)]
public class CamBam
{
    [XmlAttribute("name")]
    public string Name { get; set; } = @"CNCLib";

    [XmlAttribute("version")]
    public string Version { get; set; } = "0.9.8.0";

    [XmlType("layer")]
    public class Layer
    {
        [XmlAttribute("name")]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute("color")]
        public string Color { get; set; } = string.Empty;

        [XmlArray(ElementName = "objects")]
        public List<PLine> PLines { get; set; } = new List<PLine>();

        public PLine AddPLine()
        {
            var pline = new PLine { Id = PLines.Count + 1 };
            PLines.Add(pline);
            return pline;
        }
    }

    [XmlArray("layers")]
    public List<Layer> Layers { get; set; } = new List<Layer>();

    public Layer AddLayer()
    {
        var l = new Layer();
        Layers.Add(l);
        return l;
    }

    [XmlType("pline")]
    public class PLine
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("Closed")]
        public bool Closed { get; set; }

        [XmlArray(ElementName = "pts")]
        [XmlArrayItem(ElementName = "p")]
        public List<PLinePoints> Pts { get; set; } = new List<PLinePoints>();

        public void CheckAndSetClosed()
        {
            if (Pts.Count > 2)
            {
                if (Pts[0].CompareTo(Pts[^1]) == 0)
                {
                    Pts.RemoveAt(Pts.Count - 1);
                    Closed = true;
                    return;
                }
            }

            Closed = false;
        }
    }

    public class PLinePoints : IXmlSerializable, IComparable<PLinePoints>
    {
        public double? X  { get; set; }
        public double? Y  { get; set; }
        public double? Z  { get; set; }
        public double  X0 => X ?? 0.0;
        public double  Y0 => Y ?? 0.0;
        public double  Z0 => Z ?? 0.0;

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteString($@"{X0.ToString(CultureInfo.InvariantCulture)},{Y0.ToString(CultureInfo.InvariantCulture)},{Z0.ToString(CultureInfo.InvariantCulture)}");
        }

        public void ReadXml(XmlReader reader)
        {
            string[] fields = reader.ReadElementContentAsString().Split(',');
            if (fields.Length > 0 && !string.IsNullOrEmpty(fields[0]))
            {
                X = double.Parse(fields[0], CultureInfo.InvariantCulture);
            }
            else
            {
                X = null;
            }

            if (fields.Length > 1 && !string.IsNullOrEmpty(fields[1]))
            {
                Y = double.Parse(fields[1], CultureInfo.InvariantCulture);
            }
            else
            {
                Y = null;
            }

            if (fields.Length > 2 && !string.IsNullOrEmpty(fields[2]))
            {
                Z = double.Parse(fields[2], CultureInfo.InvariantCulture);
            }
            else
            {
                Z = null;
            }
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public int CompareTo(PLinePoints? other)
        {
            if (other != null && X == other.X && Y == other.Y && Z == other.Z)
            {
                return 0;
            }

            return 1;
        }
    }
}