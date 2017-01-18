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
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
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

		const int EEPROM_NUM_AXIS = 6;

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

		#region Axis

		public class SAxis
		{
			[DisplayName("Size")]
			[Description("Maximum size in mm/1000")]
			public uint Size { get; set; }

			[DisplayName("RefMove")]
			[Description("Reference-Move for axis")]
			public EReverenceType RefMove { get; set; }

			public override string ToString()
			{
				return Size.ToString() + (RefMove == EReverenceType.NoReference ? "" : $",{RefMove}");
			}
		};

		private SAxis[] _axis = new SAxis[EEPROM_NUM_AXIS] { new SAxis(), new SAxis(), new SAxis(), new SAxis(), new SAxis(), new SAxis() };

		public SAxis GetAxis(int axis) { return _axis[axis]; }

		[ExpandableObject]
		[Category("Axis")]
		[Description("Definition of axis")]
		public SAxis AxisX { get { return _axis[0]; } }

		[ExpandableObject]
		[Category("Axis")]
		[Description("Definition of axis")]
		public SAxis AxisY { get { return _axis[1]; } }

		[ExpandableObject]
		[Category("Axis")]
		[Description("Definition of axis")]
		public SAxis AxisZ { get { return _axis[2]; } }
		[ExpandableObject]
		[Category("Axis")]
		[Description("Definition of axis")]
		public SAxis AxisA { get { return _axis[3]; } }
		[ExpandableObject]
		[Category("Axis")]
		[Description("Definition of axis")]
		public SAxis AxisB { get { return _axis[4]; } }
		[ExpandableObject]
		[Category("Axis")]
		[Description("Definition of axis")]
		public SAxis AxisC { get { return _axis[5]; } }

		#endregion

		#region Refmove-General

		private EReverenceSequence[] _refSeqences = new EReverenceSequence[EEPROM_NUM_AXIS] { EReverenceSequence.No, EReverenceSequence.No, EReverenceSequence.No, EReverenceSequence.No, EReverenceSequence.No, EReverenceSequence.No };

		public EReverenceSequence this[int i] { get { return _refSeqences[i]; } set { _refSeqences[i] = value; } }

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 1")]
		[Description("Axis for Refeence-sequnce 1")]
		public EReverenceSequence RefSeqence1 { get { return _refSeqences[0]; } set { _refSeqences[0] = value; } }

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 2")]
		[Description("Axis for Refeence-sequnce 2")]
		public EReverenceSequence RefSeqence2 { get { return _refSeqences[1]; } set { _refSeqences[1] = value; } }

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 3")]
		[Description("Axis for Refeence-sequnce 3")]
		public EReverenceSequence RefSeqence3 { get { return _refSeqences[2]; } set { _refSeqences[2] = value; } }

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 4")]
		[Description("Axis for Refeence-sequnce 3")]
		public EReverenceSequence RefSeqence4 { get { return _refSeqences[3]; } set { _refSeqences[3] = value; } }

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 5")]
		[Description("Axis for Refeence-sequnce 5")]
		public EReverenceSequence RefSeqence5 { get { return _refSeqences[4]; } set { _refSeqences[4] = value; } }

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 6")]
		[Description("Axis for Refeence-sequnce 6")]
		public EReverenceSequence RefSeqence6 { get { return _refSeqences[5]; } set { _refSeqences[5] = value; } }

		#endregion
	}
}
