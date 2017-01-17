using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace CNCLib.Wpf.Models
{
	public class Eeprom
	{
		const string CATEGORY_INTERNAL = "Internal";
		const string CATEGORY_SIZE = "Size";
		const string CATEGORY_FEATURES = "Features";
		const string CATEGORY_PROBE = "Probe";
		const string CATEGORY_GENERAL = "General";
		const string CATEGORY_INFO = "Info";

		const string CATEGORY_AXIS_X = "Axis X";
		const string CATEGORY_AXIS_Y = "Axis Y";
		const string CATEGORY_AXIS_Z = "Axis Z";
		const string CATEGORY_AXIS_A = "Axis A";
		const string CATEGORY_AXIS_B = "Axis B";
		const string CATEGORY_AXIS_C = "Axis C";

		const int EEPROM_NUM_AXIS = 4;

		public enum EReverenceType
		{
			NoReference,
			ReferenceToMin,
			ReferenceToMax
		};
		public enum EReverenceSequence
		{
			XAxis=0,
			YAxis = 1,
			ZAxis = 2,
			AAxis = 3,
			BAxis = 4,
			CAxis = 5,
			UAxis = 6,
			VAxis = 7,
			WAxis = 8,
			No = 255
		};

		public UInt32[] Values { get; set; }

		#region General

		[Category(CATEGORY_GENERAL)]
		[DisplayName("MaxStepRate")]
		[Description("Maximum steprate in Hz")]
		public uint MaxStepRate { get; set; }

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Acc")]
		[Description("Acceleration factor")]
		public uint Acc { get; set; }

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Dec")]
		[Description("Deceleration factor")]
		public uint Dec { get; set; }

		[Category(CATEGORY_GENERAL)]
		[DisplayName("RefMoveStepRate")]
		[Description("Steprate for reference-move")]
		public uint RefMoveSteprate { get; set; }

		[Category(CATEGORY_GENERAL)]
		[DisplayName("MoveAwayFromRefernece")]
		[Description("Distance between refmove hit and 0 (in mm1000)")]
		public uint MoveAwayFromRefernece { get; set; }

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Scale mm to machine")]
		[Description("Steps for 1/1000mm => steps to go for 1/1000mm")]
		public float StepsPerMm1000 { get; set; }

		#endregion

		#region Info

		[Category(CATEGORY_INFO)]
		[DisplayName("NumAxis")]
		[Description("Supported Axis")]
		public uint NumAxis { get; set; }

		[Category(CATEGORY_INFO)]
		[DisplayName("UseAxis")]
		[Description("Useabel axis")]
		public uint UseAxis { get; set; }

		[Category(CATEGORY_INFO)]
		[DisplayName("Info1")]
		[Description("Info 32bit")]
		public uint Info1 { get; set; }

		[Category(CATEGORY_INFO)]
		[DisplayName("Info2")]
		[Description("Info 32bit")]
		public uint Info2 { get; set; }

		#endregion

		#region Axis-X

		[Category(CATEGORY_AXIS_X)]
		[DisplayName("Size-X")]
		[Description("Maximum size X in mm/1000")]
		public uint SizeX { get; set; }

		[Category(CATEGORY_AXIS_X)]
		[DisplayName("RefMove-X")]
		[Description("Reference-Move for axis X")]
		public EReverenceType RefMoveX { get; set; }

		#endregion
		#region Axis-Y

		[Category(CATEGORY_AXIS_Y)]
		[DisplayName("Size-Y")]
		[Description("Maximum size of Y in mm/1000")]
		public uint SizeY { get; set; }

		[Category(CATEGORY_AXIS_Y)]
		[DisplayName("RefMove-Y")]
		[Description("Reference-Move for axis Y")]
		public EReverenceType RefMoveY { get; set; }

		#endregion
		#region Axis-Z

		[Category(CATEGORY_AXIS_Z)]
		[DisplayName("Size-Z")]
		[Description("Maximum size of Z in mm/1000")]

		public uint SizeZ { get; set; }
		[Category(CATEGORY_AXIS_Z)]
		[DisplayName("RefMove-Z")]
		[Description("Reference-Move for axis Z")]
		public EReverenceType RefMoveZ { get; set; }

		#endregion
		#region Axis-A

		[Category(CATEGORY_AXIS_A)]
		[DisplayName("Size-A")]
		[Description("Maximum size of A in mm/1000")]
		public uint SizeA { get; set; }

		[Category(CATEGORY_AXIS_A)]
		[DisplayName("RefMove-A")]
		[Description("Reference-Move for axis A")]
		public EReverenceType RefMoveA { get; set; }

		#endregion
		#region Axis-B

		[Category(CATEGORY_AXIS_B)]
		[DisplayName("Size-B")]
		[Description("Maximum size of A in mm/1000")]
		public uint SizeB { get; set; }

		[Category(CATEGORY_AXIS_B)]
		[DisplayName("RefMove-B")]
		[Description("Reference-Move for axis B")]
		public EReverenceType RefMoveB { get; set; }

		#endregion
		#region Axis-C

		[Category(CATEGORY_AXIS_C)]
		[DisplayName("Size-C")]
		[Description("Maximum size of C in mm/1000")]
		public uint SizeC { get; set; }

		[Category(CATEGORY_AXIS_C)]
		[DisplayName("RefMove-C")]
		[Description("Reference-Move for axis C")]
		public EReverenceType RefMoveC { get; set; }

		#endregion

		#region Refmove-General

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 1")]
		[Description("Axis for Refeence-sequnce 1")]
		public EReverenceSequence RefSeqence1 { get; set; } = EReverenceSequence.No;

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 2")]
		[Description("Axis for Refeence-sequnce 2")]
		public EReverenceSequence RefSeqence2 { get; set; } = EReverenceSequence.No;

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 3")]
		[Description("Axis for Refeence-sequnce 3")]
		public EReverenceSequence RefSeqence3 { get; set; } = EReverenceSequence.No;

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 4")]
		[Description("Axis for Refeence-sequnce 3")]
		public EReverenceSequence RefSeqence4 { get; set; } = EReverenceSequence.No;

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 5")]
		[Description("Axis for Refeence-sequnce 5")]
		public EReverenceSequence RefSeqence5 { get; set; } = EReverenceSequence.No;

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 6")]
		[Description("Axis for Refeence-sequnce 6")]
		public EReverenceSequence RefSeqence6 { get; set; } = EReverenceSequence.No;

		#endregion
	}
}
