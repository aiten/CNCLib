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
using System.IO;
using System.Threading.Tasks;
using CNCLib.GCode;
using CNCLib.Wpf.Models;

namespace CNCLib.Wpf.Helpers
{
    class EepromHelper
    {
        public async Task<Eeprom> ReadEepromAsync()
        {
            UInt32[] values = await new MachineGCodeHelper().GetEpromValuesAsync(MachineGCodeHelper.DefaultEpromTimeout);
            if (values != null)
            {
                var ee = new EepromV1 { Values = values };

                if (ee.IsValid)
                {
                    File.WriteAllLines(Environment.ExpandEnvironmentVariables(@"%TEMP%\EepromRead.nc"), ee.ToGCode());
                    byte numaxis = ee[EepromV1.EValueOffsets8.NumAxis];

                    var eeprom = Eeprom.Create(ee[EepromV1.EValueOffsets32.Signatrue], numaxis);
                    eeprom.Values = values;
                    eeprom.ReadFrom(ee);

                    return eeprom;
                }
            }

            return null;
        }

        public async Task<bool> WriteEepromAsync(Eeprom EepromValue)
        {
            var ee = new EepromV1 { Values = EepromValue.Values };

            if (ee.IsValid)
            {
                EepromValue.WriteTo(ee);

                File.WriteAllLines(Environment.ExpandEnvironmentVariables(@"%TEMP%\EepromWrite.nc"), ee.ToGCode());

                await new MachineGCodeHelper().WriteEepromValuesAsync(ee);
                return true;
            }

            return false;
        }

        public async Task<bool> EraseEepromAsync()
        {
            await new MachineGCodeHelper().EraseEepromValuesAsync();
            return true;
        }
    }
}