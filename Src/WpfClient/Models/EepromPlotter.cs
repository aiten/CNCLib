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

using System.ComponentModel;

using CNCLib.GCode.Serial;

namespace CNCLib.WpfClient.Models
{
    public class EepromPlotter : Eeprom
    {
        #region Plotter

        protected const string CATEGORY_PLOTTER = "Plotter";

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

        public override void ReadFrom(EepromV1 ee)
        {
            base.ReadFrom(ee);

            PenDownFeedrate = ee[EepromV1.EValueOffsets32Plotter.EPenDownFeedrate];
            PenUpFeedrate   = ee[EepromV1.EValueOffsets32Plotter.EPenUpFeedrate];

            MovePenDownFeedrate   = ee[EepromV1.EValueOffsets32Plotter.EMovePenDownFeedrate];
            MovePenUpFeedrate     = ee[EepromV1.EValueOffsets32Plotter.EMovePenUpFeedrate];
            MovePenChangeFeedrate = ee[EepromV1.EValueOffsets32Plotter.EMovePenChangeFeedrate];

            PenDownPos = ee[EepromV1.EValueOffsets32Plotter.EPenDownPos];
            PenUpPos   = ee[EepromV1.EValueOffsets32Plotter.EPenUpPos];

            PenChangePos_x = ee[EepromV1.EValueOffsets32Plotter.EPenChangePosX];
            PenChangePos_y = ee[EepromV1.EValueOffsets32Plotter.EPenChangePosY];
            PenChangePos_z = ee[EepromV1.EValueOffsets32Plotter.EPenChangePosZ];

            PenChangePos_x_ofs = ee[EepromV1.EValueOffsets32Plotter.EPenChangePosXOfs];
            PenChangePos_y_ofs = ee[EepromV1.EValueOffsets32Plotter.EPenChangePosYOfs];

            ServoClampOpenPos  = ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampOpenPos];
            ServoClampClosePos = ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampClosePos];

            ServoClampOpenDelay  = ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampOpenDelay];
            ServoClampCloseDelay = ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampCloseDelay];
        }

        public override void WriteTo(EepromV1 ee)
        {
            base.WriteTo(ee);
            ee[EepromV1.EValueOffsets32Plotter.EPenDownFeedrate] = PenDownFeedrate;
            ee[EepromV1.EValueOffsets32Plotter.EPenUpFeedrate]   = PenUpFeedrate;

            ee[EepromV1.EValueOffsets32Plotter.EMovePenDownFeedrate]   = MovePenDownFeedrate;
            ee[EepromV1.EValueOffsets32Plotter.EMovePenUpFeedrate]     = MovePenUpFeedrate;
            ee[EepromV1.EValueOffsets32Plotter.EMovePenChangeFeedrate] = MovePenChangeFeedrate;

            ee[EepromV1.EValueOffsets32Plotter.EPenDownPos] = PenDownPos;
            ee[EepromV1.EValueOffsets32Plotter.EPenUpPos]   = PenUpPos;

            ee[EepromV1.EValueOffsets32Plotter.EPenChangePosX] = PenChangePos_x;
            ee[EepromV1.EValueOffsets32Plotter.EPenChangePosY] = PenChangePos_y;
            ee[EepromV1.EValueOffsets32Plotter.EPenChangePosZ] = PenChangePos_z;

            ee[EepromV1.EValueOffsets32Plotter.EPenChangePosXOfs] = PenChangePos_x_ofs;
            ee[EepromV1.EValueOffsets32Plotter.EPenChangePosYOfs] = PenChangePos_y_ofs;

            ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampOpenPos]  = ServoClampOpenPos;
            ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampClosePos] = ServoClampClosePos;

            ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampOpenDelay]  = ServoClampOpenDelay;
            ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampCloseDelay] = ServoClampCloseDelay;
        }
    }
}