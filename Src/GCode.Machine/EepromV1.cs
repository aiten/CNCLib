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
    using System.Collections.Generic;

    public class EepromV1 : EepromV0
    {
        #region Properties

        public override uint VersionSignature => 0x21436502;

        public const uint SIGNATURE        = 0x21436502;

        public const uint SIZEOFAXIS_EX = ((uint)EAxisOffsets32V1.InitPosition) + 1;

        public bool IsExtended => DWSizeAxis > EepromV1.SIZEOFAXIS_EX;

        public enum EValueOffsets32V1
        {
            Values8Bit1 = EValueOffsets32.FirstCustomV0,
            Values16Bit1,
            MaxStepRate,
            OffsetAccDec,
            RefMoveStepRate,
            MoveAwayFromReference,
            StepsPerMm1000
        }

        public enum EValueOffsets8V1
        {
            StepperDirection = (EValueOffsets32V1.Values8Bit1 << 8) + 0,
            SpindleFadeTime  = (EValueOffsets32V1.Values8Bit1 << 8) + 3
        }

        public enum EValueOffsets16
        {
            MaxSpindleSpeed = (EValueOffsets32V1.Values16Bit1 << 8) + 00,
            JerkSpeed       = (EValueOffsets32V1.Values16Bit1 << 8) + 01,

            Acc = (EValueOffsets32V1.OffsetAccDec << 8) + 00,
            Dec = (EValueOffsets32V1.OffsetAccDec << 8) + 01
        }

        public enum EAxisOffsets32V1
        {
            Size = 0,
            Offset1,
            InitPosition,
            MaxStepRate,
            OffsetAccDec,
            RefMoveStepRate,
            StepsPerMm1000,
            ProbeSize
        }

        public enum EAxisOffsets16V1
        {
            Acc = (EAxisOffsets32V1.OffsetAccDec << 8) + 00,
            Dec = (EAxisOffsets32V1.OffsetAccDec << 8) + 01
        }

        public enum EAxisOffsets8V1
        {
            EReverenceType        = (EAxisOffsets32V1.Offset1 << 8) + 00,
            EReverenceSequence    = (EAxisOffsets32V1.Offset1 << 8) + 1,
            EReverenceHitValueMin = (EAxisOffsets32V1.Offset1 << 8) + 2,
            EReverenceHitValueMax = (EAxisOffsets32V1.Offset1 << 8) + 3
        }

        #endregion

        #region Read/Write

        public override void ReadFrom(Eeprom eeprom)
        {
            ReadFromDefault(eeprom);

            byte numAxis = GetValue8((uint)EValueOffsets8.NumAxis);

            eeprom.NumAxis = GetValue8((uint)EValueOffsets8.NumAxis);
            eeprom.UseAxis = GetValue8((uint)EValueOffsets8.UseAxis);

            for (int i = 0; i < numAxis; i++)
            {
                ReadFromAxis(eeprom, i);
            }

            eeprom.MaxSpindleSpeed = GetValue16((uint)EValueOffsets16.MaxSpindleSpeed);
            eeprom.SpindleFadeTime = GetValue8((uint)EValueOffsets8V1.SpindleFadeTime);

            eeprom.RefMoveStepRate       = GetValue32((uint)EValueOffsets32V1.RefMoveStepRate);
            eeprom.MoveAwayFromReference = GetValue32((uint)EValueOffsets32V1.MoveAwayFromReference);

            eeprom.MaxStepRate = GetValue32((uint)EValueOffsets32V1.MaxStepRate);
            eeprom.Acc         = GetValue16((uint)EValueOffsets16.Acc);
            eeprom.Dec         = GetValue16((uint)EValueOffsets16.Dec);
            eeprom.JerkSpeed   = GetValue16((uint)EValueOffsets16.JerkSpeed);

            eeprom.StepsPerMm1000 = BitConverter.ToSingle(BitConverter.GetBytes(GetValue32((uint)EValueOffsets32V1.StepsPerMm1000)), 0);
        }

        public virtual void ReadFromAxis(Eeprom eeprom, int axis)
        {
            eeprom.GetAxis(axis).DWEESizeOf     = DWSizeAxis;
            eeprom.GetAxis(axis).Size           = GetAxisValue32(axis, (uint)EAxisOffsets32.Size);
            eeprom.GetAxis(axis).RefMove        = (Eeprom.EReverenceType)GetAxisValue8(axis, (uint)EAxisOffsets8V1.EReverenceType);
            eeprom.GetAxis(axis).RefHitValueMin = GetAxisValue8(axis, (uint)EAxisOffsets8V1.EReverenceHitValueMin);
            eeprom.GetAxis(axis).RefHitValueMax = GetAxisValue8(axis, (uint)EAxisOffsets8V1.EReverenceHitValueMax);

            eeprom.GetAxis(axis).InitPosition = GetAxisValue32(axis, (uint)EAxisOffsets32V1.InitPosition);

            eeprom.GetAxis(axis).StepperDirection = (GetValue8((uint)EValueOffsets8V1.StepperDirection) & (1 << axis)) != 0;

            eeprom[axis] = (Eeprom.EReverenceSequence)GetAxisValue8(axis, (uint)EAxisOffsets8V1.EReverenceSequence);

            if (IsExtended)
            {
                eeprom.GetAxis(axis).MaxStepRate     = GetAxisValue32(axis, (uint)EAxisOffsets32V1.MaxStepRate);
                eeprom.GetAxis(axis).Acc             = GetAxisValue16(axis, (uint)EAxisOffsets16V1.Acc);
                eeprom.GetAxis(axis).Dec             = GetAxisValue16(axis, (uint)EAxisOffsets16V1.Dec);
                eeprom.GetAxis(axis).StepsPerMm1000  = BitConverter.ToSingle(BitConverter.GetBytes(GetAxisValue32(axis, (uint)EAxisOffsets32V1.StepsPerMm1000)), 0);
                eeprom.GetAxis(axis).ProbeSize       = GetAxisValue32(axis, (uint)EAxisOffsets32V1.ProbeSize);
                eeprom.GetAxis(axis).RefMoveStepRate = GetAxisValue32(axis, (uint)EAxisOffsets32V1.RefMoveStepRate);
            }
        }

        public override void WriteTo(Eeprom eeprom)
        {
            byte numAxis = GetValue8((uint)EValueOffsets8.NumAxis);

            for (int i = 0; i < numAxis; i++)
            {
                WriteToAxis(eeprom, i);
            }

            SetValue16((uint)EValueOffsets16.MaxSpindleSpeed, eeprom.MaxSpindleSpeed);
            SetValue8((uint)EValueOffsets8V1.SpindleFadeTime, eeprom.SpindleFadeTime);

            SetValue32((uint)EValueOffsets32V1.RefMoveStepRate,       eeprom.RefMoveStepRate);
            SetValue32((uint)EValueOffsets32V1.MoveAwayFromReference, eeprom.MoveAwayFromReference);

            SetValue32((uint)EValueOffsets32V1.MaxStepRate, eeprom.MaxStepRate);
            SetValue16((uint)EValueOffsets16.Acc,       eeprom.Acc);
            SetValue16((uint)EValueOffsets16.Dec,       eeprom.Dec);
            SetValue16((uint)EValueOffsets16.JerkSpeed, eeprom.JerkSpeed);

            SetValue32((uint)EValueOffsets32V1.StepsPerMm1000, BitConverter.ToUInt32(BitConverter.GetBytes(eeprom.StepsPerMm1000), 0));
        }

        public void WriteToAxis(Eeprom eeprom, int i)
        {
            SetAxisValue32(i, (uint)EAxisOffsets32.Size, eeprom.GetAxis(i).Size);
            SetAxisValue8(i, (uint)EAxisOffsets8V1.EReverenceType,        (byte)eeprom.GetAxis(i).RefMove);
            SetAxisValue8(i, (uint)EAxisOffsets8V1.EReverenceSequence,    (byte)(Eeprom.EReverenceSequence)eeprom[i]);
            SetAxisValue8(i, (uint)EAxisOffsets8V1.EReverenceHitValueMin, eeprom.GetAxis(i).RefHitValueMin);
            SetAxisValue8(i, (uint)EAxisOffsets8V1.EReverenceHitValueMax, eeprom.GetAxis(i).RefHitValueMax);

            int direction = GetValue8((uint)EValueOffsets8V1.StepperDirection) & (~(1 << i));
            if (eeprom.GetAxis(i).StepperDirection)
            {
                direction += 1 << i;
            }

            SetValue8((uint)EValueOffsets8V1.StepperDirection, (byte)direction);

            SetAxisValue32(i, (uint)EAxisOffsets32V1.InitPosition, eeprom.GetAxis(i).InitPosition);

            if (IsExtended)
            {
                SetAxisValue32(i, (uint)EAxisOffsets32V1.MaxStepRate, eeprom.GetAxis(i).MaxStepRate);
                SetAxisValue16(i, (uint)EAxisOffsets16V1.Acc, eeprom.GetAxis(i).Acc);
                SetAxisValue16(i, (uint)EAxisOffsets16V1.Dec, eeprom.GetAxis(i).Dec);
                SetAxisValue32(i, (uint)EAxisOffsets32V1.StepsPerMm1000,  BitConverter.ToUInt32(BitConverter.GetBytes(eeprom.GetAxis(i).StepsPerMm1000), 0));
                SetAxisValue32(i, (uint)EAxisOffsets32V1.ProbeSize,       eeprom.GetAxis(i).ProbeSize);
                SetAxisValue32(i, (uint)EAxisOffsets32V1.RefMoveStepRate, eeprom.GetAxis(i).RefMoveStepRate);
            }
        }
    }

    #endregion
}