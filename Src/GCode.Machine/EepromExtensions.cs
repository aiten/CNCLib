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

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace CNCLib.GCode.Machine
{
    public static class EepromExtensions
    {
        public static Eeprom ConvertEeprom(UInt32[] values)
        {
            var ee = new EepromV1 { Values = values };

            if (ee.IsValid)
            {
                File.WriteAllLines(Environment.ExpandEnvironmentVariables(@"%TEMP%\EepromRead.nc"), ee.ToGCode());

                var eeprom = Create(ee[EepromV1.EValueOffsets32.Signature]);
                eeprom.Values = values;
                eeprom.ReadFrom(ee);

                return eeprom;
            }

            return null;
        }

        public static Eeprom Create(uint signature)
        {
            if (signature == EepromV1.SIGNATUREPLOTTER)
            {
                return new EepromPlotter();
            }

            return new Eeprom();
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
    }
}