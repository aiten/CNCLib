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

namespace CNCLib.GCode.Machine;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class Eeprom
{
    public const string CATEGORY_INTERNAL = "Internal";
    public const string CATEGORY_SIZE     = "Size";
    public const string CATEGORY_FEATURES = "Features";
    public const string CATEGORY_PROBE    = "Probe";
    public const string CATEGORY_GENERAL  = "General";
    public const string CATEGORY_INFO     = "Info";
    public const string CATEGORY_PLOTTER  = "Plotter";

    protected const int EEPROM_NUM_AXIS = 6;

    public enum ECommandSyntax
    {
        GCodeBasic = 0,
        GCode      = 1,
        Grbl       = 2,
        Hpgl       = 7 // max 3 bit
    }

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

    [Category(CATEGORY_GENERAL)]
    [DisplayName("StepperOffTimeout")]
    [Description("Time in sec the stepper remain active (enabled) after a stop")]
    public ushort StepperOffTimeout { get; set; }

    #endregion

    #region Info

    [Category(CATEGORY_INFO)]
    [DisplayName("Signature")]
    [Description("Signature"), ReadOnly(true)]
    public uint Signature { get; set; }

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
    [Description("Capability of machine commands")]
    public ECommandSyntax CommandSyntax
    {
        get => (ECommandSyntax)EepromV1.GetCommandSyntax(Info1);
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

    [Category(CATEGORY_INFO)]
    [DisplayName("WorkOffsetCount")]
    [Description("Count of work offsets (G54-??)")]
    public uint WorkOffsetCount
    {
        get => EepromV1.GetWorkOffsetCount(Info1);
        set { }
    }

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
            return Size.ToString() + (RefMove == EReverenceType.NoReference ? "" : $",{RefMove}");
        }
    };

    protected SAxis[] _axis = new SAxis[EEPROM_NUM_AXIS] { new SAxis(), new SAxis(), new SAxis(), new SAxis(), new SAxis(), new SAxis() };

    public SAxis GetAxis(int axis)
    {
        return _axis[axis];
    }

    //[ExpandableObject]
    [Category("Axis")]
    [DisplayName("Axis-X")]
    [Description("Definition of axis")]
    public SAxis AxisX => _axis[0];

    //[ExpandableObject]
    [Category("Axis")]
    [DisplayName("Axis-Y")]
    [Description("Definition of axis")]
    public SAxis AxisY => _axis[1];

    //[ExpandableObject]
    [Category("Axis")]
    [DisplayName("Axis-Z")]
    [Description("Definition of axis")]
    public SAxis AxisZ => _axis[2];

    //[ExpandableObject]
    [Category("Axis")]
    [DisplayName("Axis-A")]
    [Description("Definition of axis")]
    public SAxis AxisA => _axis[3];

    //[ExpandableObject]
    [Category("Axis")]
    [DisplayName("Axis-B")]
    [Description("Definition of axis")]
    public SAxis AxisB => _axis[4];

    //[ExpandableObject]
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

    #region Plotter

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("PenDownFeedrate")]
    [Description("Default drawing speed, in mm1000/min")]
    public uint PenDownFeedrate { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("PenUpFeedrate")]
    [Description("Default traveling speed, in mm1000/min, reduced to maxsteprate")]
    public uint PenUpFeedrate { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("MovePenDownFeedrate")]
    [Description("Z-axis speed to set pen, in mm1000/min, reduced to maxsteprate - if servo, delay in ms e.g. 200 for 0.2 sec")]
    public uint MovePenDownFeedrate { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("MovePenUpFeedrate")]
    [Description("Z-axis speed to rise pen, in mm1000/min, reduced to maxsteprate - if servo, delay in ms e.g. 200 for 0.2 sec")]
    public uint MovePenUpFeedrate { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("MovePenChangeFeedrate")]
    [Description("Z-axis speed while pen is changed, in mm1000/min, reduced to maxsteprate - if servo, delay in ms e.g. 200 for 0.2 sec")]
    public uint MovePenChangeFeedrate { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("PenDownPos")]
    [Description("Z-axis position of pen down, in mm1000, adjusted to 0..zmax")]
    public uint PenDownPos { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("PenUpPos")]
    [Description("Z-axis position of pen up, in mm1000, adjusted to 0..zmax")]
    public uint PenUpPos { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("PenChangePosX")]
    [Description("X-axis position for pen change, in mm1000, adjusted to 0..xmax")]
    public uint PenChangePos_x { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("PenChangePosY")]
    [Description("Y-axis position for pen change, in mm1000, adjusted to 0..ymax")]
    public uint PenChangePos_y { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("PenChangePosZ")]
    [Description("Z-axis position for pen change, in mm1000, adjusted to 0..zmax")]
    public uint PenChangePos_z { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("PenChangePosX_Ofs")]
    [Description("X-axis distance between pens in pen-stack, in mm1000")]
    public uint PenChangePos_x_ofs { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("PenChangePosY_Ofs")]
    [Description("Y-axis distance between pens in pen-stack, in mm1000")]
    public uint PenChangePos_y_ofs { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("ServoClampOpenPos")]
    [Description("Clamp open servo pos, in micro seconds, values 1000..2000")]
    public ushort ServoClampOpenPos { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("ServoClampClosePos")]
    [Description("Clamp close servo pos, in micro seconds, values 1000..2000")]
    public ushort ServoClampClosePos { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("ServoClampOpenDelay")]
    [Description("Delay to open clamp, in milli seconds, 1000 = 1sec)")]
    public ushort ServoClampOpenDelay { get; set; }

    [Category(CATEGORY_PLOTTER)]
    [DisplayName("ServoClampCloseDelay")]
    [Description("Delay to close clamp, in milli seconds, 1000 = 1sec)")]
    public ushort ServoClampCloseDelay { get; set; }

    #endregion
}