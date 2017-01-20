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
		public static Eeprom Create(int numaxis)
		{
			if (numaxis > 4) return new EepromAxis6();
			if (numaxis == 4) return new EepromAxis4();
			if (numaxis == 3) return new EepromAxis3();
			if (numaxis >= 1) return new EepromAxis2();

			return new Eeprom();
		}

		protected const string CATEGORY_INTERNAL = "Internal";
		protected const string CATEGORY_SIZE = "Size";
		protected const string CATEGORY_FEATURES = "Features";
		protected const string CATEGORY_PROBE = "Probe";
		protected const string CATEGORY_GENERAL = "General";
		protected const string CATEGORY_INFO = "Info";

		protected const int EEPROM_NUM_AXIS = 6;

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
		public ushort Acc { get; set; }

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Dec")]
		[Description("Deceleration factor")]
		public ushort Dec { get; set; }

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

		[Category(CATEGORY_GENERAL)]
		[DisplayName("MaxSpindleSpeed")]
		[Description("Max speed (rpm) of spindle")]
		public ushort MaxSpindleSpeed { get; set; }

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
		public GCode.EepromV1.EInfo1 Info1 { get; set; }

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

			[DisplayName("StepperDirection")]
			[Description("Invert the rotation direction of the stepper")]
			public bool StepperDirection { get; set; }

			public override string ToString()
			{
				return Size.ToString() + (RefMove == EReverenceType.NoReference ? "" : $",{RefMove}");
			}
		};

		protected SAxis[] _axis = new SAxis[EEPROM_NUM_AXIS] { new SAxis(), new SAxis(), new SAxis(), new SAxis(), new SAxis(), new SAxis() };

		public SAxis GetAxis(int axis) { return _axis[axis]; }

		#endregion

		#region Refmove-General

		protected EReverenceSequence[] _refSeqences = new EReverenceSequence[EEPROM_NUM_AXIS] { EReverenceSequence.No, EReverenceSequence.No, EReverenceSequence.No, EReverenceSequence.No, EReverenceSequence.No, EReverenceSequence.No };

		public EReverenceSequence this[int i] { get { return _refSeqences[i]; } set { _refSeqences[i] = value; } }

		#endregion
	}
}
