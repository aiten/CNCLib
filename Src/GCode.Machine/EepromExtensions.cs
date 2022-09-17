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
    using System.ComponentModel;
    using System.IO;

    public static class EepromExtensions
    {
        public static Eeprom ConvertEeprom(UInt32[] values)
        {
            if (values.Length > 0)
            {
                var eeprom        = new Eeprom();
                var machinEerprom = CreateMachineEeprom(values);

                eeprom.Values = values;
                machinEerprom.ReadFrom(eeprom);

                File.WriteAllLines(Environment.ExpandEnvironmentVariables(@"%TEMP%\EepromRead.nc"), machinEerprom.ToGCode());

                return eeprom;
            }

            return null;
        }

        public static EepromV0 CreateMachineEeprom(UInt32[] values)
        {
            var signature = values[(int)EepromV0.EValueOffsets32.Signature];
            switch (signature)
            {
                case EepromV1.SIGNATURE:               return new EepromV1() { Values        = values };
                case EepromV1Plotter.SIGNATUREPLOTTER: return new EepromV1Plotter() { Values = values };
                case EepromV2.SIGNATURE:               return new EepromV2() { Values        = values };
                case EepromV2Plotter.SIGNATUREPLOTTER: return new EepromV2Plotter() { Values = values };
            }

            return null;
        }

        public static bool? IsPropertyBrowsable(this Eeprom eeprom, PropertyDescriptor property)
        {
            string propertyName = property.Name;
            bool   isAxis       = property.ComponentType.Name == nameof(Eeprom.SAxis);

            if (isAxis)
            {
                if (eeprom.GetAxis(0).DWEESizeOf <= EepromV1.SIZEOFAXIS_EX)
                {
                    switch (propertyName)
                    {
                        case nameof(Eeprom.SAxis.Acc):
                        case nameof(Eeprom.SAxis.Dec):
                        case nameof(Eeprom.SAxis.MaxStepRate):
                        case nameof(Eeprom.SAxis.StepsPerMm1000):
                        case nameof(Eeprom.SAxis.ProbeSize):
                        case nameof(Eeprom.SAxis.RefMoveStepRate):
                            return false;
                    }
                }
            }
            else
            {
                var signature = eeprom.Signature;

                if (signature != EepromV1Plotter.SIGNATUREPLOTTER)
                {
                    var               attributes  = property.Attributes;
                    CategoryAttribute myAttribute = (CategoryAttribute)attributes[typeof(CategoryAttribute)];
                    if (myAttribute.Category == Eeprom.CATEGORY_PLOTTER)
                    {
                        return false;
                    }
                }

                if (signature == EepromV1.SIGNATURE || signature == EepromV1Plotter.SIGNATUREPLOTTER)
                {
                    if (propertyName == nameof(Eeprom.StepperOffTimeout))
                    {
                        return false;
                    }
                }

                if (propertyName == "Values")
                {
                    return false;
                }

                if (propertyName == nameof(Eeprom.AxisY) || propertyName == nameof(Eeprom.RefSequence2))
                {
                    return eeprom.NumAxis >= 2;
                }

                if (propertyName == nameof(Eeprom.AxisZ) || propertyName == nameof(Eeprom.RefSequence3))
                {
                    return eeprom.NumAxis >= 3;
                }

                if (propertyName == nameof(Eeprom.AxisA) || propertyName == nameof(Eeprom.RefSequence4))
                {
                    return eeprom.NumAxis >= 4;
                }

                if (propertyName == nameof(Eeprom.AxisB) || propertyName == nameof(Eeprom.RefSequence5))
                {
                    return eeprom.NumAxis >= 5;
                }

                if (propertyName == nameof(Eeprom.AxisC) || propertyName == nameof(Eeprom.RefSequence6))
                {
                    return eeprom.NumAxis >= 6;
                }
            }

            return null;
        }
        /*
                public static void ReadFrom(this Eeprom eeprom, EepromV1 ee)
                {
                    eeprom.Signature = ee[EepromV1.EValueOffsets32.Signature];

                    byte numAxis = ee[EepromV1.EValueOffsets8.NumAxis];

                    eeprom.NumAxis = ee[EepromV1.EValueOffsets8.NumAxis];
                    eeprom.UseAxis = ee[EepromV1.EValueOffsets8.UseAxis];

                    eeprom.Info1 = ee[EepromV1.EValueOffsets32.Info1];
                    eeprom.Info2 = ee[EepromV1.EValueOffsets32.Info2];

                    for (int i = 0; i < numAxis; i++)
                    {
                        eeprom.GetAxis(i).DWEESizeOf     = ee.DWSizeAxis;
                        eeprom.GetAxis(i).Size           = ee[i, EepromV1.EAxisOffsets32.Size];
                        eeprom.GetAxis(i).RefMove        = (Eeprom.EReverenceType)ee[i, EepromV1.EAxisOffsets8.EReverenceType];
                        eeprom.GetAxis(i).RefHitValueMin = ee[i, EepromV1.EAxisOffsets8.EReverenceHitValueMin];
                        eeprom.GetAxis(i).RefHitValueMax = ee[i, EepromV1.EAxisOffsets8.EReverenceHitValueMax];

                        eeprom.GetAxis(i).InitPosition = ee[i, EepromV1.EAxisOffsets32.InitPosition];

                        eeprom.GetAxis(i).StepperDirection = (ee[EepromV1.EValueOffsets8.StepperDirection] & (1 << i)) != 0;

                        eeprom[i] = (Eeprom.EReverenceSequence)ee[i, EepromV1.EAxisOffsets8.EReverenceSequence];

                        if (ee.DWSizeAxis > EepromV1.SIZEOFAXIS_EX)
                        {
                            eeprom.GetAxis(i).MaxStepRate     = ee[i, EepromV1.EAxisOffsets32.MaxStepRate];
                            eeprom.GetAxis(i).Acc             = ee[i, EepromV1.EAxisOffsets16.Acc];
                            eeprom.GetAxis(i).Dec             = ee[i, EepromV1.EAxisOffsets16.Dec];
                            eeprom.GetAxis(i).StepsPerMm1000  = BitConverter.ToSingle(BitConverter.GetBytes(ee[i, EepromV1.EAxisOffsets32.StepsPerMm1000]), 0);
                            eeprom.GetAxis(i).ProbeSize       = ee[i, EepromV1.EAxisOffsets32.ProbeSize];
                            eeprom.GetAxis(i).RefMoveStepRate = ee[i, EepromV1.EAxisOffsets32.RefMoveStepRate];
                        }
                    }

                    eeprom.MaxSpindleSpeed = ee[EepromV1.EValueOffsets16.MaxSpindleSpeed];
                    eeprom.SpindleFadeTime = ee[EepromV1.EValueOffsets8.SpindleFadeTime];

                    eeprom.RefMoveStepRate       = ee[EepromV1.EValueOffsets32.RefMoveStepRate];
                    eeprom.MoveAwayFromReference = ee[EepromV1.EValueOffsets32.MoveAwayFromReference];

                    eeprom.MaxStepRate = ee[EepromV1.EValueOffsets32.MaxStepRate];
                    eeprom.Acc         = ee[EepromV1.EValueOffsets16.Acc];
                    eeprom.Dec         = ee[EepromV1.EValueOffsets16.Dec];
                    eeprom.JerkSpeed   = ee[EepromV1.EValueOffsets16.JerkSpeed];

                    eeprom.StepsPerMm1000 = BitConverter.ToSingle(BitConverter.GetBytes(ee[EepromV1.EValueOffsets32.StepsPerMm1000]), 0);

                    if (eeprom.Signature == EepromV1.SIGNATUREPLOTTER)
                    {
                        eeprom.PenDownFeedrate = ee[EepromV1.EValueOffsets32Plotter.EPenDownFeedrate];
                        eeprom.PenUpFeedrate   = ee[EepromV1.EValueOffsets32Plotter.EPenUpFeedrate];

                        eeprom.MovePenDownFeedrate   = ee[EepromV1.EValueOffsets32Plotter.EMovePenDownFeedrate];
                        eeprom.MovePenUpFeedrate     = ee[EepromV1.EValueOffsets32Plotter.EMovePenUpFeedrate];
                        eeprom.MovePenChangeFeedrate = ee[EepromV1.EValueOffsets32Plotter.EMovePenChangeFeedrate];

                        eeprom.PenDownPos = ee[EepromV1.EValueOffsets32Plotter.EPenDownPos];
                        eeprom.PenUpPos   = ee[EepromV1.EValueOffsets32Plotter.EPenUpPos];

                        eeprom.PenChangePos_x = ee[EepromV1.EValueOffsets32Plotter.EPenChangePosX];
                        eeprom.PenChangePos_y = ee[EepromV1.EValueOffsets32Plotter.EPenChangePosY];
                        eeprom.PenChangePos_z = ee[EepromV1.EValueOffsets32Plotter.EPenChangePosZ];

                        eeprom.PenChangePos_x_ofs = ee[EepromV1.EValueOffsets32Plotter.EPenChangePosXOfs];
                        eeprom.PenChangePos_y_ofs = ee[EepromV1.EValueOffsets32Plotter.EPenChangePosYOfs];

                        eeprom.ServoClampOpenPos  = ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampOpenPos];
                        eeprom.ServoClampClosePos = ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampClosePos];

                        eeprom.ServoClampOpenDelay  = ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampOpenDelay];
                        eeprom.ServoClampCloseDelay = ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampCloseDelay];
                    }
                }

                public static void WriteTo(this Eeprom eeprom, EepromV1 ee)
                {
                    byte numAxis = ee[EepromV1.EValueOffsets8.NumAxis];

                    for (int i = 0; i < numAxis; i++)
                    {
                        ee[i, EepromV1.EAxisOffsets32.Size]                 = eeprom.GetAxis(i).Size;
                        ee[i, EepromV1.EAxisOffsets8.EReverenceType]        = (byte)eeprom.GetAxis(i).RefMove;
                        ee[i, EepromV1.EAxisOffsets8.EReverenceSequence]    = (byte)(Eeprom.EReverenceSequence)eeprom[i];
                        ee[i, EepromV1.EAxisOffsets8.EReverenceHitValueMin] = eeprom.GetAxis(i).RefHitValueMin;
                        ee[i, EepromV1.EAxisOffsets8.EReverenceHitValueMax] = eeprom.GetAxis(i).RefHitValueMax;

                        int direction = ee[EepromV1.EValueOffsets8.StepperDirection] & (~(1 << i));
                        if (eeprom.GetAxis(i).StepperDirection)
                        {
                            direction += 1 << i;
                        }

                        ee[EepromV1.EValueOffsets8.StepperDirection] = (byte)direction;

                        ee[i, EepromV1.EAxisOffsets32.InitPosition] = eeprom.GetAxis(i).InitPosition;

                        if (ee.DWSizeAxis > EepromV1.SIZEOFAXIS_EX)
                        {
                            ee[i, EepromV1.EAxisOffsets32.MaxStepRate]     = eeprom.GetAxis(i).MaxStepRate;
                            ee[i, EepromV1.EAxisOffsets16.Acc]             = eeprom.GetAxis(i).Acc;
                            ee[i, EepromV1.EAxisOffsets16.Dec]             = eeprom.GetAxis(i).Dec;
                            ee[i, EepromV1.EAxisOffsets32.StepsPerMm1000]  = BitConverter.ToUInt32(BitConverter.GetBytes(eeprom.GetAxis(i).StepsPerMm1000), 0);
                            ee[i, EepromV1.EAxisOffsets32.ProbeSize]       = eeprom.GetAxis(i).ProbeSize;
                            ee[i, EepromV1.EAxisOffsets32.RefMoveStepRate] = eeprom.GetAxis(i).RefMoveStepRate;
                        }
                    }

                    ee[EepromV1.EValueOffsets16.MaxSpindleSpeed] = eeprom.MaxSpindleSpeed;
                    ee[EepromV1.EValueOffsets8.SpindleFadeTime]  = eeprom.SpindleFadeTime;

                    ee[EepromV1.EValueOffsets32.RefMoveStepRate]       = eeprom.RefMoveStepRate;
                    ee[EepromV1.EValueOffsets32.MoveAwayFromReference] = eeprom.MoveAwayFromReference;

                    ee[EepromV1.EValueOffsets32.MaxStepRate] = eeprom.MaxStepRate;
                    ee[EepromV1.EValueOffsets16.Acc]         = eeprom.Acc;
                    ee[EepromV1.EValueOffsets16.Dec]         = eeprom.Dec;
                    ee[EepromV1.EValueOffsets16.JerkSpeed]   = eeprom.JerkSpeed;

                    ee[EepromV1.EValueOffsets32.StepsPerMm1000] = BitConverter.ToUInt32(BitConverter.GetBytes(eeprom.StepsPerMm1000), 0);

                    if (ee[EepromV1.EValueOffsets32.Signature] == EepromV1.SIGNATUREPLOTTER)
                    {
                        ee[EepromV1.EValueOffsets32Plotter.EPenDownFeedrate] = eeprom.PenDownFeedrate;
                        ee[EepromV1.EValueOffsets32Plotter.EPenUpFeedrate]   = eeprom.PenUpFeedrate;

                        ee[EepromV1.EValueOffsets32Plotter.EMovePenDownFeedrate]   = eeprom.MovePenDownFeedrate;
                        ee[EepromV1.EValueOffsets32Plotter.EMovePenUpFeedrate]     = eeprom.MovePenUpFeedrate;
                        ee[EepromV1.EValueOffsets32Plotter.EMovePenChangeFeedrate] = eeprom.MovePenChangeFeedrate;

                        ee[EepromV1.EValueOffsets32Plotter.EPenDownPos] = eeprom.PenDownPos;
                        ee[EepromV1.EValueOffsets32Plotter.EPenUpPos]   = eeprom.PenUpPos;

                        ee[EepromV1.EValueOffsets32Plotter.EPenChangePosX] = eeprom.PenChangePos_x;
                        ee[EepromV1.EValueOffsets32Plotter.EPenChangePosY] = eeprom.PenChangePos_y;
                        ee[EepromV1.EValueOffsets32Plotter.EPenChangePosZ] = eeprom.PenChangePos_z;

                        ee[EepromV1.EValueOffsets32Plotter.EPenChangePosXOfs] = eeprom.PenChangePos_x_ofs;
                        ee[EepromV1.EValueOffsets32Plotter.EPenChangePosYOfs] = eeprom.PenChangePos_y_ofs;

                        ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampOpenPos]  = eeprom.ServoClampOpenPos;
                        ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampClosePos] = eeprom.ServoClampClosePos;

                        ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampOpenDelay]  = eeprom.ServoClampOpenDelay;
                        ee[EepromV1.EValueOffsets16Plotter.EPenChangeServoClampCloseDelay] = eeprom.ServoClampCloseDelay;
                    }
                }
        */
    }
}