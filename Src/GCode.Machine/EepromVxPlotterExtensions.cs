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

namespace CNCLib.GCode.Machine
{
    public static class EepromVxPlotterExtensions
    {
        #region Properties

        public enum EValueOffsets32Plotter
        {
            EPenDownFeedrate,
            EPenUpFeedrate,

            EMovePenDownFeedrate,
            EMovePenUpFeedrate,
            EMovePenChangeFeedrate,

            EPenDownPos,
            EPenUpPos,

            EPenChangePosX,
            EPenChangePosY,
            EPenChangePosZ,

            EPenChangePosXOfs,
            EPenChangePosYOfs,

            EPenChangeServoClampPos,
            EPenChangeServoClampDelay
        }

        public enum EValueOffsets16Plotter
        {
            EPenChangeServoClampOpenPos  = (EValueOffsets32Plotter.EPenChangeServoClampPos << 8) + 00,
            EPenChangeServoClampClosePos = (EValueOffsets32Plotter.EPenChangeServoClampPos << 8) + 01,

            EPenChangeServoClampOpenDelay  = (EValueOffsets32Plotter.EPenChangeServoClampDelay << 8) + 00,
            EPenChangeServoClampCloseDelay = (EValueOffsets32Plotter.EPenChangeServoClampDelay << 8) + 01
        }

        #endregion

        #region Read/Write

        private static uint AddPlotterOfs16(EepromV1 ee, uint ofs)
        {
            return ((uint)ofs + (ee.OfsAfterAxis << 8));
        }

        public static uint GetPlotterValue32(EepromV1 ee, uint ofs)
        {
            return ee.Values[(int)ofs + ee.OfsAfterAxis];
        }

        public static ushort GetPlotterValue16(EepromV1 ee, uint ofs)
        {
            return ee.GetValue16(AddPlotterOfs16(ee, ofs));
        }

        public static void ReadPlotter(this EepromV1 ee, Eeprom eeprom)
        {

            eeprom.PenDownFeedrate = GetPlotterValue32(ee, (uint)EValueOffsets32Plotter.EPenDownFeedrate);
            eeprom.PenUpFeedrate   = GetPlotterValue32(ee, (uint)EValueOffsets32Plotter.EPenUpFeedrate);

            eeprom.MovePenDownFeedrate   = GetPlotterValue32(ee, (uint)EValueOffsets32Plotter.EMovePenDownFeedrate);
            eeprom.MovePenUpFeedrate     = GetPlotterValue32(ee, (uint)EValueOffsets32Plotter.EMovePenUpFeedrate);
            eeprom.MovePenChangeFeedrate = GetPlotterValue32(ee, (uint)EValueOffsets32Plotter.EMovePenChangeFeedrate);

            eeprom.PenDownPos = GetPlotterValue32(ee, (uint)EValueOffsets32Plotter.EPenDownPos);
            eeprom.PenUpPos   = GetPlotterValue32(ee, (uint)EValueOffsets32Plotter.EPenUpPos);

            eeprom.PenChangePos_x = GetPlotterValue32(ee, (uint)EValueOffsets32Plotter.EPenChangePosX);
            eeprom.PenChangePos_y = GetPlotterValue32(ee, (uint)EValueOffsets32Plotter.EPenChangePosY);
            eeprom.PenChangePos_z = GetPlotterValue32(ee, (uint)EValueOffsets32Plotter.EPenChangePosZ);

            eeprom.PenChangePos_x_ofs = GetPlotterValue32(ee, (uint)EValueOffsets32Plotter.EPenChangePosXOfs);
            eeprom.PenChangePos_y_ofs = GetPlotterValue32(ee, (uint)EValueOffsets32Plotter.EPenChangePosYOfs);

            eeprom.ServoClampOpenPos  = GetPlotterValue16(ee, (uint)EValueOffsets16Plotter.EPenChangeServoClampOpenPos);
            eeprom.ServoClampClosePos = GetPlotterValue16(ee, (uint)EValueOffsets16Plotter.EPenChangeServoClampClosePos);

            eeprom.ServoClampOpenDelay  = GetPlotterValue16(ee, (uint)EValueOffsets16Plotter.EPenChangeServoClampOpenDelay);
            eeprom.ServoClampCloseDelay = GetPlotterValue16(ee, (uint)EValueOffsets16Plotter.EPenChangeServoClampCloseDelay);
        }

        public static void SetPlotterValue32(EepromV1 ee, uint ofs, uint value)
        {
            ee.Values[(int)ofs + ee.OfsAfterAxis] = value;
        }

        public static void SetPlotterValue16(EepromV1 ee, uint ofs, ushort value)
        {
            ee.SetValue16(AddPlotterOfs16(ee, ofs), value);
        }

        public static void WritePlotter(this EepromV1 ee, Eeprom eeprom)
        {
            SetPlotterValue32(ee,(uint)EValueOffsets32Plotter.EPenDownFeedrate, eeprom.PenDownFeedrate);
            SetPlotterValue32(ee,(uint)EValueOffsets32Plotter.EPenUpFeedrate,   eeprom.PenUpFeedrate);

            SetPlotterValue32(ee,(uint)EValueOffsets32Plotter.EMovePenDownFeedrate,   eeprom.MovePenDownFeedrate);
            SetPlotterValue32(ee,(uint)EValueOffsets32Plotter.EMovePenUpFeedrate,     eeprom.MovePenUpFeedrate);
            SetPlotterValue32(ee,(uint)EValueOffsets32Plotter.EMovePenChangeFeedrate, eeprom.MovePenChangeFeedrate);

            SetPlotterValue32(ee,(uint)EValueOffsets32Plotter.EPenDownPos, eeprom.PenDownPos);
            SetPlotterValue32(ee,(uint)EValueOffsets32Plotter.EPenUpPos,   eeprom.PenUpPos);

            SetPlotterValue32(ee,(uint)EValueOffsets32Plotter.EPenChangePosX, eeprom.PenChangePos_x);
            SetPlotterValue32(ee,(uint)EValueOffsets32Plotter.EPenChangePosY, eeprom.PenChangePos_y);
            SetPlotterValue32(ee,(uint)EValueOffsets32Plotter.EPenChangePosZ, eeprom.PenChangePos_z);

            SetPlotterValue32(ee,(uint)EValueOffsets32Plotter.EPenChangePosXOfs, eeprom.PenChangePos_x_ofs);
            SetPlotterValue32(ee,(uint)EValueOffsets32Plotter.EPenChangePosYOfs, eeprom.PenChangePos_y_ofs);

            SetPlotterValue16(ee,(uint)EValueOffsets16Plotter.EPenChangeServoClampOpenPos,  eeprom.ServoClampOpenPos);
            SetPlotterValue16(ee,(uint)EValueOffsets16Plotter.EPenChangeServoClampClosePos, eeprom.ServoClampClosePos);

            SetPlotterValue16(ee,(uint)EValueOffsets16Plotter.EPenChangeServoClampOpenDelay,  eeprom.ServoClampOpenDelay);
            SetPlotterValue16(ee,(uint)EValueOffsets16Plotter.EPenChangeServoClampCloseDelay, eeprom.ServoClampCloseDelay);
        }

        #endregion
    }
}