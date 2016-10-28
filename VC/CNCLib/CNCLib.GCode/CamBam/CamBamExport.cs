using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Framework.Tools.Drawing;

namespace CNCLib.GCode.CamBam
{
	public class CamBamExport
	{
		[XmlType("layer")]
		public class Layer
		{
			public String Name { get; set; }
			public System.Drawing.Color Color { get; set; }

			public List<Object> Objects { get; set; } = new List<Object>();
		};

		public List<Layer> Lasers { get; set; } = new List<Layer>();

		public class Object
		{
			public int Id { get; set; }
		};

		public class PLine : Object
		{
			public bool Closed { get; set; }

			public List<Point3D> Pts { get; set; }
		} 
	}
}
