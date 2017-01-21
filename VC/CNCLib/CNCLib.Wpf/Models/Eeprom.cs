////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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
using System.ComponentModel;
using CNCLib.GCode;

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
		[Description("Max speed (rpm) of spindle or laser power (1-255)")]
		public ushort MaxSpindleSpeed { get; set; }

		#endregion

		#region Info

		[Category(CATEGORY_INFO)]
		[DisplayName("NumAxis")]
		[Description("Supported Axis"), ReadOnly(true)]
		public uint NumAxis { get; set; }

		[Category(CATEGORY_INFO)]
		[DisplayName("UseAxis")]
		[Description("Useabel axis"), ReadOnly(true)]
		public uint UseAxis { get; set; }

		[Category(CATEGORY_INFO)]
		[DisplayName("Info1")]
		[Description("Info 32bit"), ReadOnly(true)]
		public uint Info1 { get; set; }

		[Category(CATEGORY_INFO)]
		[DisplayName("HasSpindle")]
		[Description("Maschine has a spindle, can use m3/m5")]
		public bool HasSpindle { get { return (((EepromV1.EInfo1) Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_SPINDLE)); } set { } }

		[Category(CATEGORY_INFO)]
		[DisplayName("AnalogSpindle")]
		[Description("Can set the speed of the spindle with e.g.  m3 s1000")]
		public bool HasAnalogSpindle { get { return (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_SPINDLE_ANALOG)); } set { } }

		[Category(CATEGORY_INFO)]
		[DisplayName("HasSpindleDirection")]
		[Description("Can set spindle direction, mse m3/m4")]
		public bool HasSpindleDirection { get { return (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_SPINDLE_DIR)); } set { } }

		[Category(CATEGORY_INFO)]
		[DisplayName("HasCoolant")]
		[Description("Machine has coolant (use m7/m9)")]
		public bool HasCoolant { get { return (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_COOLANT)); } set { } }

        [Category(CATEGORY_INFO)]
		[DisplayName("HasProbe")]
		[Description("Machine has probe input (use g31)")]
		public bool HasProbe { get { return (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_PROBE)); } set { } }

		[Category(CATEGORY_INFO)]
		[DisplayName("HasSD")]
		[Description("Machine has a SD card")]
		public bool HasSD { get { return (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_SD)); } set { } }

		[Category(CATEGORY_INFO)]
		[DisplayName("CanRotate")]
		[Description("Machine can rotate coordinate system (g68/g69)")]
		public bool CanRotate { get { return (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_SD)); } set { } }

		[Category(CATEGORY_INFO)]
		[DisplayName("HasHold")]
		[Description("Machine has a hold input")]
		public bool HasHold { get { return (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_HOLD)); } set { } }

		[Category(CATEGORY_INFO)]
		[DisplayName("HasResume")]
		[Description("Machine has a resume input")]
		public bool HasResume { get { return (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_RESUME)); } set { } }

		[Category(CATEGORY_INFO)]
		[DisplayName("HasHoldResume")]
		[Description("Machine has a hold/resume input")]
		public bool HasHoldResume { get { return (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_HOLDRESUME)); } set { } }

		[Category(CATEGORY_INFO)]
		[DisplayName("HasKill")]
		[Description("Machine has a kill input")]
		public bool HasKill { get { return (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_KILL)); } set { } }

		[Category(CATEGORY_INFO)]
		[DisplayName("IsLaser")]
		[Description("Machine is a laser")]
		public bool IsLaser { get { return (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_LASER)); } set { } }

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

			[DisplayName("RefHitValue")]
			[Description("Value of IO if reference is hit - usual 0, optical 1")]
			public byte RefHitValue { get; set; }

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
