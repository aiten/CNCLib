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

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using CNCLib.GCode;
using CNCLib.Logic.Contract.DTO;

using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace CNCLib.WpfClient.Models
{
    public class Eeprom
    {
        public static Eeprom Create(uint signature, int numAxis)
        {
            if (signature == EepromV1.SIGNATUREPLOTTER)
            {
                return new EepromPlotter();
            }

            return new Eeprom();
        }

        public bool? IsPropertyBrowsable(PropertyDescriptor property)
        {
            string propertyName = property.Name;
            bool   isAxis       = property.ComponentType.Name == nameof(SAxis);

            if (isAxis)
            {
                if (GetAxis(0).DWEESizeOf <= EepromV1.SIZEOFAXIS_EX)
                {
                    switch (propertyName)
                    {
                        case nameof(SAxis.Acc):
                        case nameof(SAxis.Dec):
                        case nameof(SAxis.MaxStepRate):
                        case nameof(SAxis.StepsPerMm1000):
                        case nameof(SAxis.ProbeSize):
                        case nameof(SAxis.RefMoveStepRate):
                            return false;
                    }
                }
            }
            else
            {
                if (propertyName == "Values")
                {
                    return false;
                }

                if (propertyName == nameof(AxisY) || propertyName == nameof(RefSequence2))
                {
                    return NumAxis >= 2;
                }

                if (propertyName == nameof(AxisZ) || propertyName == nameof(RefSequence3))
                {
                    return NumAxis >= 3;
                }

                if (propertyName == nameof(AxisA) || propertyName == nameof(RefSequence4))
                {
                    return NumAxis >= 4;
                }

                if (propertyName == nameof(AxisB) || propertyName == nameof(RefSequence5))
                {
                    return NumAxis >= 5;
                }

                if (propertyName == nameof(AxisC) || propertyName == nameof(RefSequence6))
                {
                    return NumAxis >= 6;
                }
            }

            return null;
        }

        protected const string CATEGORY_INTERNAL = "Internal";
        protected const string CATEGORY_SIZE     = "Size";
        protected const string CATEGORY_FEATURES = "Features";
        protected const string CATEGORY_PROBE    = "Probe";
        protected const string CATEGORY_GENERAL  = "General";
        protected const string CATEGORY_INFO     = "Info";

        protected const int EEPROM_NUM_AXIS = 6;

        public enum EReverenceType
        {
            NoReference,
            ReferenceToMin,
            ReferenceToMax
        };

        public enum EReverenceSequence
        {
            XAxis = 0,
            YAxis = 1,
            ZAxis = 2,
            AAxis = 3,
            BAxis = 4,
            CAxis = 5,
            UAxis = 6,
            VAxis = 7,
            WAxis = 8,
            No    = 255
        };

        public uint[] Values { get; set; }

        #region General

        [Range(1, int.MaxValue)]
        [Category(CATEGORY_GENERAL)]
        [DisplayName("MaxStepRate")]
        [Description("Maximum steprate in Hz (AVR 8bit max 16bit, e.g. 25000)")]
        public uint MaxStepRate { get; set; }

        [Range(62, 1024)]
        [Category(CATEGORY_GENERAL)]
        [DisplayName("Acc")]
        [Description("Acceleration factor (e.g. 350)], must be > 61")]
        public ushort Acc { get; set; }

        [Range(62, 1024)]
        [Category(CATEGORY_GENERAL)]
        [DisplayName("Dec")]
        [Description("Deceleration factor (e.g. 400), must be > 61")]
        public ushort Dec { get; set; }

        [Category(CATEGORY_GENERAL)]
        [DisplayName("JerkSpeed")]
        [Description("Maximum Jerkspeed - speed difference without acceleration - in Hz (e.g. 1000)")]
        public ushort JerkSpeed { get; set; }

        [Category(CATEGORY_GENERAL)]
        [DisplayName("RefMoveStepRate")]
        [Description("Steprate for reference-move (AVR 8bit max 16bit, less than 'MaxStepRate')")]
        public uint RefMoveStepRate { get; set; }

        [Category(CATEGORY_GENERAL)]
        [DisplayName("MoveAwayFromReference")]
        [Description("Distance between refmove hit and 0 (in mm1000)")]
        public uint MoveAwayFromReference { get; set; }

        [Category(CATEGORY_GENERAL)]
        [DisplayName("Scale mm to machine")]
        [Description("Steps for 1/1000mm => steps to go for 1/1000mm")]
        public float StepsPerMm1000 { get; set; }

        [Category(CATEGORY_GENERAL)]
        [DisplayName("MaxSpindleSpeed")]
        [Description("Max speed (rpm) of spindle or laser power (1-255)")]
        public ushort MaxSpindleSpeed { get; set; }

        [Category(CATEGORY_GENERAL)]
        [DisplayName("SpindleFadeTime")]
        [Description("Time in ms between incrementing the analog PWM output - e.g. 8ms will result in a 2040ms time between stop an max")]
        public byte SpindleFadeTime { get; set; }

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
        public bool HasSpindle
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_SPINDLE));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("AnalogSpindle")]
        [Description("Can set the speed of the spindle with e.g.  m3 s1000")]
        public bool HasAnalogSpindle
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_SPINDLE_ANALOG));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("HasSpindleDirection")]
        [Description("Can set spindle direction, mse m3/m4")]
        public bool HasSpindleDirection
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_SPINDLE_DIR));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("HasCoolant")]
        [Description("Machine has coolant (use m7/m9)")]
        public bool HasCoolant
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_COOLANT));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("HasProbe")]
        [Description("Machine has probe input (use g31)")]
        public bool HasProbe
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_PROBE));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("HasSD")]
        [Description("Machine has a SD card")]
        public bool HasSD
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_SD));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("HasEeprom")]
        [Description("Configuration can be written to eeprom")]
        public bool HasEeprom
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_EEPROM));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("CanRotate")]
        [Description("Machine can rotate coordinate system (g68/g69)")]
        public bool CanRotate
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_ROTATE));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("HasHold")]
        [Description("Machine has a hold input")]
        public bool HasHold
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_HOLD));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("HasResume")]
        [Description("Machine has a resume input")]
        public bool HasResume
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_RESUME));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("HasHoldResume")]
        [Description("Machine has a hold/resume input")]
        public bool HasHoldResume
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_HOLDRESUME));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("HasKill")]
        [Description("Machine has a kill input")]
        public bool HasKill
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_KILL));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("IsLaser")]
        [Description("Machine is a laser")]
        public bool IsLaser
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_LASER));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("CommandSyntax")]
        [Description("Capability of macine commands")]
        public CommandSyntax CommandSyntax
        {
            get => EepromV1.GetCommandSyntax(Info1);
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("DtrIsReset")]
        [Description("For Arduino Uno, Mega, ... Dtr cause a reset when connecting. For a Arduino zero Dtr must be set/used to transfer data (no reset)")]
        public bool DtrIsReset
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_DTR_IS_RESET));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("Need EEprom Flush")]
        [Description("EEprom Flush command must be executed to save to EEprom (Arduino zero)")]
        public bool NeedEEpromFlush
        {
            get => (((EepromV1.EInfo1)Info1).HasFlag(EepromV1.EInfo1.EEPROM_INFO_NEED_EEPROM_FLUSH));
            set { }
        }

        [Category(CATEGORY_INFO)]
        [DisplayName("Info2")]
        [Description("Info 32bit"), ReadOnly(true)]
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

            [DisplayName("RefHitValueMin")]
            [Description("Value of IO if reference is hit - usual 0, optical 1, 255 disabled")]
            public byte RefHitValueMin { get; set; }

            [DisplayName("RefHitValueMax")]
            [Description("Value of IO if reference is hit - usual 0, optical 1, 255 disabled")]
            public byte RefHitValueMax { get; set; }

            [Range(1, int.MaxValue)]
            [DisplayName("MaxStepRate")]
            [Description("Maximum steprate in Hz (AVR 8bit max 16bit, e.g. 25000), 0 for machine default")]
            public uint MaxStepRate { get; set; }

            [Range(62, 1024)]
            [DisplayName("Acc")]
            [Description("Acceleration factor (e.g. 350)], must be > 61, 0 for machine default")]
            public ushort Acc { get; set; }

            [Range(62, 1024)]
            [DisplayName("Dec")]
            [Description("Deceleration factor (e.g. 400), must be > 61, 0 for machine default")]
            public ushort Dec { get; set; }

            [DisplayName("Scale mm to machine")]
            [Description("Steps for 1/1000mm => steps to go for 1/1000mm, 0 for machine default")]
            public float StepsPerMm1000 { get; set; }

            [DisplayName("RefMoveStepRate")]
            [Description("Steprate for reference-move (AVR 8bit max 16bit, less than 'MaxStepRate'), 0 for machine default")]
            public uint RefMoveStepRate { get; set; }

            [DisplayName("Init-Position")]
            [Description("Position (in mm/1000) while startup. The reference move will overwrite this position.")]
            public uint InitPosition { get; set; }

            [DisplayName("ProbeSize")]
            [Description("Default probe size in mm/1000 (used in Lcd)")]
            public uint ProbeSize { get; set; }

            [Browsable(false)]
            public uint DWEESizeOf { get; set; }

            public override string ToString()
            {
                return Size.ToString() + (RefMove == EReverenceType.NoReference?"":$",{RefMove}");
            }
        };

        protected SAxis[] _axis = new SAxis[EEPROM_NUM_AXIS] { new SAxis(), new SAxis(), new SAxis(), new SAxis(), new SAxis(), new SAxis() };

        public SAxis GetAxis(int axis)
        {
            return _axis[axis];
        }

        [ExpandableObject]
        [Category("Axis")]
        [DisplayName("Axis-X")]
        [Description("Definition of axis")]
        public SAxis AxisX => _axis[0];

        [ExpandableObject]
        [Category("Axis")]
        [DisplayName("Axis-Y")]
        [Description("Definition of axis")]
        public SAxis AxisY => _axis[1];

        [ExpandableObject]
        [Category("Axis")]
        [DisplayName("Axis-Z")]
        [Description("Definition of axis")]
        public SAxis AxisZ => _axis[2];

        [ExpandableObject]
        [Category("Axis")]
        [DisplayName("Axis-A")]
        [Description("Definition of axis")]
        public SAxis AxisA => _axis[3];

        [ExpandableObject]
        [Category("Axis")]
        [DisplayName("Axis-B")]
        [Description("Definition of axis")]
        public SAxis AxisB => _axis[4];

        [ExpandableObject]
        [Category("Axis")]
        [DisplayName("Axis-C")]
        [Description("Definition of axis")]
        public SAxis AxisC => _axis[5];

        [Category(CATEGORY_GENERAL)]
        [DisplayName("Ref-Sequence 1")]
        [Description("Axis for reference-sequence 1")]
        public EReverenceSequence RefSequence1
        {
            get => _refSequences[0];
            set => _refSequences[0] = value;
        }

        [Category(CATEGORY_GENERAL)]
        [DisplayName("Ref-Sequence 2")]
        [Description("Axis for reference-sequence 2")]
        public EReverenceSequence RefSequence2
        {
            get => _refSequences[1];
            set => _refSequences[1] = value;
        }

        [Category(CATEGORY_GENERAL)]
        [DisplayName("Ref-Sequence 3")]
        [Description("Axis for reference-sequence 3")]
        public EReverenceSequence RefSequence3
        {
            get => _refSequences[2];
            set => _refSequences[2] = value;
        }

        [Category(CATEGORY_GENERAL)]
        [DisplayName("Ref-Sequence 4")]
        [Description("Axis for reference-sequence 4")]
        public EReverenceSequence RefSequence4
        {
            get => _refSequences[3];
            set => _refSequences[3] = value;
        }

        [Category(CATEGORY_GENERAL)]
        [DisplayName("Ref-Sequence 5")]
        [Description("Axis for reference-sequence 5")]
        public EReverenceSequence RefSequence5
        {
            get => _refSequences[4];
            set => _refSequences[4] = value;
        }

        [Category(CATEGORY_GENERAL)]
        [DisplayName("Ref-Sequence 6")]
        [Description("Axis for reference-sequence 6")]
        public EReverenceSequence RefSequence6
        {
            get => _refSequences[5];
            set => _refSequences[5] = value;
        }

        #endregion

        #region Refmove-General

        protected EReverenceSequence[] _refSequences = new EReverenceSequence[EEPROM_NUM_AXIS]
        {
            EReverenceSequence.No, EReverenceSequence.No, EReverenceSequence.No, EReverenceSequence.No, EReverenceSequence.No, EReverenceSequence.No
        };

        public EReverenceSequence this[int i]
        {
            get => _refSequences[i];
            set => _refSequences[i] = value;
        }

        #endregion

        #region overrides

        public virtual void ReadFrom(EepromV1 ee)
        {
            byte numAxis = ee[EepromV1.EValueOffsets8.NumAxis];

            NumAxis = ee[EepromV1.EValueOffsets8.NumAxis];
            UseAxis = ee[EepromV1.EValueOffsets8.UseAxis];

            Info1 = ee[EepromV1.EValueOffsets32.Info1];
            Info2 = ee[EepromV1.EValueOffsets32.Info2];

            for (int i = 0; i < numAxis; i++)
            {
                GetAxis(i).DWEESizeOf     = ee.DWSizeAxis;
                GetAxis(i).Size           = ee[i, EepromV1.EAxisOffsets32.Size];
                GetAxis(i).RefMove        = (EReverenceType)ee[i, EepromV1.EAxisOffsets8.EReverenceType];
                GetAxis(i).RefHitValueMin = ee[i, EepromV1.EAxisOffsets8.EReverenceHitValueMin];
                GetAxis(i).RefHitValueMax = ee[i, EepromV1.EAxisOffsets8.EReverenceHitValueMax];

                GetAxis(i).InitPosition   = ee[i, EepromV1.EAxisOffsets32.InitPosition];

                GetAxis(i).StepperDirection = (ee[EepromV1.EValueOffsets8.StepperDirection] & (1 << i)) != 0;

                this[i] = (EReverenceSequence)ee[i, EepromV1.EAxisOffsets8.EReverenceSequence];

                if (ee.DWSizeAxis > EepromV1.SIZEOFAXIS_EX)
                {
                    GetAxis(i).MaxStepRate     = ee[i, EepromV1.EAxisOffsets32.MaxStepRate];
                    GetAxis(i).Acc             = ee[i, EepromV1.EAxisOffsets16.Acc];
                    GetAxis(i).Dec             = ee[i, EepromV1.EAxisOffsets16.Dec];
                    GetAxis(i).StepsPerMm1000  = BitConverter.ToSingle(BitConverter.GetBytes(ee[i, EepromV1.EAxisOffsets32.StepsPerMm1000]), 0);
                    GetAxis(i).ProbeSize       = ee[i, EepromV1.EAxisOffsets32.ProbeSize];
                    GetAxis(i).RefMoveStepRate = ee[i, EepromV1.EAxisOffsets32.RefMoveStepRate];
                }
            }

            MaxSpindleSpeed = ee[EepromV1.EValueOffsets16.MaxSpindleSpeed];
            SpindleFadeTime = ee[EepromV1.EValueOffsets8.SpindleFadeTime];

            RefMoveStepRate       = ee[EepromV1.EValueOffsets32.RefMoveStepRate];
            MoveAwayFromReference = ee[EepromV1.EValueOffsets32.MoveAwayFromReference];

            MaxStepRate = ee[EepromV1.EValueOffsets32.MaxStepRate];
            Acc         = ee[EepromV1.EValueOffsets16.Acc];
            Dec         = ee[EepromV1.EValueOffsets16.Dec];
            JerkSpeed   = ee[EepromV1.EValueOffsets16.JerkSpeed];

            StepsPerMm1000 = BitConverter.ToSingle(BitConverter.GetBytes(ee[EepromV1.EValueOffsets32.StepsPerMm1000]), 0);
        }

        public virtual void WriteTo(EepromV1 ee)
        {
            byte numAxis = ee[EepromV1.EValueOffsets8.NumAxis];

            for (int i = 0; i < numAxis; i++)
            {
                ee[i, EepromV1.EAxisOffsets32.Size]                 = GetAxis(i).Size;
                ee[i, EepromV1.EAxisOffsets8.EReverenceType]        = (byte)GetAxis(i).RefMove;
                ee[i, EepromV1.EAxisOffsets8.EReverenceSequence]    = (byte)(EReverenceSequence)this[i];
                ee[i, EepromV1.EAxisOffsets8.EReverenceHitValueMin] = GetAxis(i).RefHitValueMin;
                ee[i, EepromV1.EAxisOffsets8.EReverenceHitValueMax] = GetAxis(i).RefHitValueMax;

                int direction = ee[EepromV1.EValueOffsets8.StepperDirection] & (~(1 << i));
                if (GetAxis(i).StepperDirection)
                {
                    direction += 1 << i;
                }

                ee[EepromV1.EValueOffsets8.StepperDirection] = (byte)direction;

                ee[i, EepromV1.EAxisOffsets32.InitPosition] = GetAxis(i).InitPosition;

                if (ee.DWSizeAxis > EepromV1.SIZEOFAXIS_EX)
                {
                    ee[i, EepromV1.EAxisOffsets32.MaxStepRate]     = GetAxis(i).MaxStepRate;
                    ee[i, EepromV1.EAxisOffsets16.Acc]             = GetAxis(i).Acc;
                    ee[i, EepromV1.EAxisOffsets16.Dec]             = GetAxis(i).Dec;
                    ee[i, EepromV1.EAxisOffsets32.StepsPerMm1000]  = BitConverter.ToUInt32(BitConverter.GetBytes(GetAxis(i).StepsPerMm1000), 0);
                    ee[i, EepromV1.EAxisOffsets32.ProbeSize]       = GetAxis(i).ProbeSize;
                    ee[i, EepromV1.EAxisOffsets32.RefMoveStepRate] = GetAxis(i).RefMoveStepRate;
                }
            }

            ee[EepromV1.EValueOffsets16.MaxSpindleSpeed] = MaxSpindleSpeed;
            ee[EepromV1.EValueOffsets8.SpindleFadeTime]  = SpindleFadeTime;

            ee[EepromV1.EValueOffsets32.RefMoveStepRate]       = RefMoveStepRate;
            ee[EepromV1.EValueOffsets32.MoveAwayFromReference] = MoveAwayFromReference;

            ee[EepromV1.EValueOffsets32.MaxStepRate] = MaxStepRate;
            ee[EepromV1.EValueOffsets16.Acc]         = Acc;
            ee[EepromV1.EValueOffsets16.Dec]         = Dec;
            ee[EepromV1.EValueOffsets16.JerkSpeed]   = JerkSpeed;

            ee[EepromV1.EValueOffsets32.StepsPerMm1000] = BitConverter.ToUInt32(BitConverter.GetBytes(StepsPerMm1000), 0);
        }

        #endregion
    }
}