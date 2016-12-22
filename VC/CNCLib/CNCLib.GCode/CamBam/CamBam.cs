////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CNCLib.GCode.CamBam
{
	[XmlRootAttribute("CADFile",IsNullable = false)]
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
			public string Name { get; set; }
			[XmlAttribute("color")]
			public string Color { get; set; }

			[XmlArray(ElementName = "objects")]
			public List<PLine> PLines { get; set; } = new List<PLine>();
	
			public PLine AddPLine()
			{
				var pline = new PLine();
				pline.Id = PLines.Count + 1;
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
					if (Pts[0].CompareTo(Pts[Pts.Count-1]) == 0)
					{
						Pts.RemoveAt(Pts.Count - 1);
						Closed = true;
						return;
					}
				}
				Closed = false;
			}
		}

		public class PLinePoints :IXmlSerializable, IComparable<PLinePoints>
		{
			public double? X { get; set; }
			public double? Y { get; set; }
			public double? Z { get; set; }

			public void WriteXml(XmlWriter writer)
			{
				writer.WriteString(
				string.Format(@"{0},{1},{2}",
				(X ?? 0).ToString(CultureInfo.InvariantCulture),
				(Y ?? 0).ToString(CultureInfo.InvariantCulture),
				(Z ?? 0).ToString(CultureInfo.InvariantCulture)));
			}

			public void ReadXml(XmlReader reader)
			{
				string[] fields = reader.ReadElementContentAsString().Split(',');
				if (fields.Length > 0 && !string.IsNullOrEmpty(fields[0])) X = double.Parse(fields[0], CultureInfo.InvariantCulture); else X = null;
				if (fields.Length > 1 && !string.IsNullOrEmpty(fields[1])) Y = double.Parse(fields[1], CultureInfo.InvariantCulture); else Y = null;
				if (fields.Length > 2 && !string.IsNullOrEmpty(fields[2])) Z = double.Parse(fields[2], CultureInfo.InvariantCulture); else Z = null;
			}

			public XmlSchema GetSchema()
			{
				return null;
			}

			public int CompareTo(PLinePoints other)
			{
				if ((X == other.X) && (Y == other.Y) && (Z == other.Z))
					return 0;

				return 1;
			}
		}
	}
}
