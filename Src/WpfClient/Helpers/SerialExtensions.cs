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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;

using CNCLib.GCode;
using CNCLib.GCode.Serial;
using CNCLib.WpfClient.Models;

using Framework.Arduino.SerialCommunication;
using Framework.Arduino.SerialCommunication.Abstraction;

using Machine = CNCLib.Logic.Abstraction.DTO.Machine;

namespace CNCLib.WpfClient.Helpers
{
    public static class SerialExtension
    {
        #region Probe

        public const int DefaultProbeTimeout = 15000;

        public static async Task<bool> SendProbeCommandAsync(this ISerial serial, Machine machine, int axisIndex)
        {
            string  axisName  = machine.GetAxisName(axisIndex);
            decimal probeSize = machine.GetProbeSize(axisIndex);

            string probDist   = machine.ProbeDist.ToString(CultureInfo.InvariantCulture);
            string probDistUp = machine.ProbeDistUp.ToString(CultureInfo.InvariantCulture);
            string probFeed   = machine.ProbeFeed.ToString(CultureInfo.InvariantCulture);

            var result = await serial.SendCommandAsync("g91 g31 " + axisName + "-" + probDist + " F" + probFeed + " g90", DefaultProbeTimeout);
            if (result?.LastOrDefault()?.ReplyType.HasFlag(EReplyType.ReplyError) == false)
            {
                serial.QueueCommand("g92 " + axisName + (-probeSize).ToString(CultureInfo.InvariantCulture));
                serial.QueueCommand("g91 g0" + axisName + probDistUp + " g90");
                return true;
            }

            return false;
        }

        #endregion

        #region EEprom

        public const int DefaultEpromTimeout = 3000;

        public static async Task<uint[]> GetEpromValuesAsync(this ISerial serial, int waitForMilliseconds)
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
                        if (intValues.ContainsKey(i))
                        {
                            ret[i] = intValues[i];
                        }
                    }

                    return ret;
                }
            }

            return null;
        }

        public static async Task WriteEepromValuesAsync(this ISerial serial, EepromV1 ee)
        {
            await serial.SendCommandAsync(@"$!", DefaultEpromTimeout);
            await serial.SendCommandsAsync(ee.ToGCode(), DefaultEpromTimeout);
        }

        public static async Task EraseEepromValuesAsync(this ISerial serial)
        {
            await serial.SendCommandAsync(@"$!",   DefaultEpromTimeout);
            await serial.SendCommandAsync(@"$0=0", DefaultEpromTimeout);
        }

        public static async Task<Eeprom> ReadEepromAsync(this ISerial serial)
        {
            uint[] values = await serial.GetEpromValuesAsync(SerialExtension.DefaultEpromTimeout);
            if (values != null)
            {
                var ee = new EepromV1 { Values = values };

                if (ee.IsValid)
                {
                    File.WriteAllLines(Environment.ExpandEnvironmentVariables(@"%TEMP%\EepromRead.nc"), ee.ToGCode());
                    byte numAxis = ee[EepromV1.EValueOffsets8.NumAxis];

                    var eeprom = Eeprom.Create(ee[EepromV1.EValueOffsets32.Signature], numAxis);
                    eeprom.Values = values;
                    eeprom.ReadFrom(ee);

                    return eeprom;
                }
            }

            return null;
        }

        public static async Task<bool> WriteEepromAsync(this ISerial serial, Eeprom eepromValue)
        {
            var ee = new EepromV1 { Values = eepromValue.Values };

            if (ee.IsValid)
            {
                eepromValue.WriteTo(ee);

                File.WriteAllLines(Environment.ExpandEnvironmentVariables(@"%TEMP%\EepromWrite.nc"), ee.ToGCode());

                await serial.WriteEepromValuesAsync(ee);
                return true;
            }

            return false;
        }

        public static async Task<bool> EraseEepromAsync(this ISerial serial)
        {
            await serial.EraseEepromValuesAsync();
            return true;
        }

        #endregion

        #region send/queue

        public const int DefaultTimeout = 120 * 1000;

        public static void PrepareAndQueueCommand(this ISerial serial, Machine machine, string commandString)
        {
            serial.QueueCommand(machine.PrepareCommand(commandString));
        }

        public static async Task SendMacroCommandAsync(this ISerial serial, Machine machine, string commandString)
        {
            string[] separators = { @"\n" };
            string[] cmds       = commandString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in cmds)
            {
                string[] infos = s.Split(':');
                int      axis;

                if (infos.Length > 1 && string.Compare(infos[0], @";probe", true) == 0 && -1 != (axis = GCodeHelper.AxisNameToIndex(infos[1])))
                {
                    if (false == await serial.SendProbeCommandAsync(machine, axis))
                    {
                        break;
                    }
                }
                else if (infos.Length == 1 && string.Compare(infos[0], @";beep", true) == 0)
                {
                    SystemSounds.Beep.Play();
                }
                else
                {
                    if (s.TrimEnd().EndsWith("?"))
                    {
                        var result = await serial.SendCommandAsync(s.TrimEnd().TrimEnd('?'), DefaultTimeout);
                        if (result?.LastOrDefault()?.ReplyType.HasFlag(EReplyType.ReplyError) == false)
                        {
                            return;
                        }
                    }
                    else
                    {
                        await serial.SendCommandAsync(s, DefaultTimeout);
                    }
                }
            }
        }

        #endregion
    }
}