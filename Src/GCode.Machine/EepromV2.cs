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
    using System;

    public class EepromV2 : EepromV1
    {
        public const uint SIGNATURE = 0x21436503;

        #region Properties

        public override uint VersionSignature => 0x21436503;


        public enum EValueOffsets32V2
        {
            Values8Bit1 = EValueOffsets32.FirstCustomV0,
            Values16Bit1,
            MaxStepRate,
            OffsetAccDec,
            RefMoveStepRate,
            MoveAwayFromReference,
            StepperOffTimeout_Dummy16_2,
            Dummy16_3_4,
            Dummy32_1,
            Dummy32_2,
            StepsPerMm1000
        }

        public enum EValueOffsets8V2
        {
            StepperDirection = (EValueOffsets32V2.Values8Bit1 << 8) + 0,
            SpindleFadeTime  = (EValueOffsets32V2.Values8Bit1 << 8) + 3
        }

        public enum EValueOffsets16
        {
            MaxSpindleSpeed = (EValueOffsets32V2.Values16Bit1 << 8) + 00,
            JerkSpeed       = (EValueOffsets32V2.Values16Bit1 << 8) + 01,

            Acc = (EValueOffsets32V2.OffsetAccDec << 8) + 00,
            Dec = (EValueOffsets32V2.OffsetAccDec << 8) + 01,

            StepperTimeout = (EValueOffsets32V2.StepperOffTimeout_Dummy16_2 << 8) + 00,
            Dummy16_2      = (EValueOffsets32V2.StepperOffTimeout_Dummy16_2 << 8) + 01
        }

        #endregion

        #region Read/Write

        public override void ReadFrom(Eeprom eeprom)
        {
            base.ReadFromDefault(eeprom);

            byte numAxis = GetValue8((uint)EValueOffsets8.NumAxis);

            eeprom.NumAxis = GetValue8((uint)EValueOffsets8.NumAxis);
            eeprom.UseAxis = GetValue8((uint)EValueOffsets8.UseAxis);

            for (int i = 0; i < numAxis; i++)
            {
                ReadFromAxis(eeprom, i);
            }

            eeprom.MaxSpindleSpeed = GetValue16((uint)EValueOffsets16.MaxSpindleSpeed);
            eeprom.SpindleFadeTime = GetValue8((uint)EValueOffsets8V2.SpindleFadeTime);

            eeprom.RefMoveStepRate       = GetValue32((uint)EValueOffsets32V2.RefMoveStepRate);
            eeprom.MoveAwayFromReference = GetValue32((uint)EValueOffsets32V2.MoveAwayFromReference);

            eeprom.MaxStepRate = GetValue32((uint)EValueOffsets32V2.MaxStepRate);
            eeprom.Acc         = GetValue16((uint)EValueOffsets16.Acc);
            eeprom.Dec         = GetValue16((uint)EValueOffsets16.Dec);
            eeprom.JerkSpeed   = GetValue16((uint)EValueOffsets16.JerkSpeed);

            eeprom.StepperOffTimeout = GetValue16((uint)EValueOffsets16.StepperTimeout);

            eeprom.StepsPerMm1000 = BitConverter.ToSingle(BitConverter.GetBytes(GetValue32((uint)EValueOffsets32V2.StepsPerMm1000)), 0);
        }

        public override void WriteTo(Eeprom eeprom)
        {
            byte numAxis = GetValue8((uint)EValueOffsets8.NumAxis);

            for (int i = 0; i < numAxis; i++)
            {
                WriteToAxis(eeprom, i);
            }

            SetValue16((uint)EValueOffsets16.MaxSpindleSpeed, eeprom.MaxSpindleSpeed);
            SetValue8((uint)EValueOffsets8V2.SpindleFadeTime, eeprom.SpindleFadeTime);

            SetValue32((uint)EValueOffsets32V2.RefMoveStepRate,       eeprom.RefMoveStepRate);
            SetValue32((uint)EValueOffsets32V2.MoveAwayFromReference, eeprom.MoveAwayFromReference);

            SetValue32((uint)EValueOffsets32V2.MaxStepRate, eeprom.MaxStepRate);
            SetValue16((uint)EValueOffsets16.Acc,            eeprom.Acc);
            SetValue16((uint)EValueOffsets16.Dec,            eeprom.Dec);
            SetValue16((uint)EValueOffsets16.JerkSpeed,      eeprom.JerkSpeed);

            SetValue16((uint)EValueOffsets16.StepperTimeout, eeprom.StepperOffTimeout);

            SetValue32((uint)EValueOffsets32V2.StepsPerMm1000, BitConverter.ToUInt32(BitConverter.GetBytes(eeprom.StepsPerMm1000), 0));
        }
    }

    #endregion
}