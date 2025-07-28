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

namespace CNCLib.GCode.Serial;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.GCode.Machine;

using Framework.Arduino.SerialCommunication;
using Framework.Arduino.SerialCommunication.Abstraction;

public static class GCodeSerialEepromExtension
{
    public static async Task<uint[]?> GetEpromValues(this ISerial serial, int waitForMilliseconds = GCodeSerial.DefaultEpromTimeout)
    {
        var cmd = (await serial.SendCommandAsync("$?", waitForMilliseconds)).FirstOrDefault();
        if (cmd != null && string.IsNullOrEmpty(cmd.ResultText) == false)
        {
            string[] separators = { "\n", "\r" };
            string[] lines      = cmd.ResultText.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            var      intValues  = new Dictionary<int, uint>();
            int      maxSlot    = -1;
            foreach (var line in lines)
            {
                // e.g. $1=65535(ffff)
                string[] assign = line.Split('=');

                int slot;
                if (assign.Length == 2 && assign[0].StartsWith("$") && int.TryParse(assign[0].TrimStart('$'), out slot))
                {
                    uint   slotValue;
                    string valueStr = assign[1];
                    int    idx1     = valueStr.IndexOf('(');
                    if (idx1 > 0)
                    {
                        valueStr = valueStr.Substring(0, idx1);
                    }

                    if (uint.TryParse(valueStr, out slotValue))
                    {
                        intValues[slot] = slotValue;
                        if (maxSlot < slot)
                        {
                            maxSlot = slot;
                        }
                    }
                }
            }

            if (maxSlot > 0)
            {
                var ret = new uint[maxSlot + 1];
                for (int i = 0; i <= maxSlot; i++)
                {
                    if (intValues.TryGetValue(i, out var value))
                    {
                        ret[i] = value;
                    }
                }

                return ret;
            }
        }

        return null;
    }

    public static async Task WriteEepromValues(this ISerial serial, EepromV0 ee)
    {
        await serial.SendCommandAsync(@"$!", GCodeSerial.DefaultEpromTimeout);
        await serial.SendCommandsAsync(ee.ToGCode(), GCodeSerial.DefaultEpromTimeout);
        await serial.SendCommandAsync(@"$w", GCodeSerial.DefaultEpromTimeout);
    }

    public static async Task EraseEepromValues(this ISerial serial)
    {
        await serial.SendCommandAsync(@"$!",   GCodeSerial.DefaultEpromTimeout);
        await serial.SendCommandAsync(@"$0=0", GCodeSerial.DefaultEpromTimeout);
        await serial.SendCommandAsync(@"$w", GCodeSerial.DefaultEpromTimeout);
    }

    public static async Task<Eeprom?> ReadEeprom(this ISerial serial)
    {
        var values = await serial.GetEpromValues(GCodeSerial.DefaultEpromTimeout);
        return EepromExtensions.ConvertEeprom(values);
    }

    public static async Task<bool> WriteEeprom(this ISerial serial, Eeprom eepromValue)
    {
        var ee = EepromExtensions.CreateMachineEeprom(eepromValue.Values);

        if (ee != null && ee.IsValid)
        {
            ee.WriteTo(eepromValue);

            await File.WriteAllLinesAsync(Environment.ExpandEnvironmentVariables(@"%TEMP%\EepromWrite.nc"), ee.ToGCode());

            await serial.WriteEepromValues(ee);
            return true;
        }

        return false;
    }

    public static async Task<bool> EraseEeprom(this ISerial serial)
    {
        await serial.EraseEepromValues();
        return true;
    }

    #region Convert

    #endregion
}