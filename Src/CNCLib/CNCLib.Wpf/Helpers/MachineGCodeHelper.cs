////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;

using Framework.Arduino.SerialCommunication;

using CNCLib.GCode;
using CNCLib.Logic.Contracts.DTO;

using System.Media;
using System.Collections.Generic;

namespace CNCLib.Wpf.Helpers
{
    public class MachineGCodeHelper
    {
        #region Probe

        public async Task<bool> SendProbeCommandAsync(int axisIndex)
        {
            return await SendProbeCommandAsync(Global.Instance.Machine, axisIndex);
        }

        public async Task<bool> SendProbeCommandAsync(Machine machine, int axisIndex)
        {
            string  axisName  = machine.GetAxisName(axisIndex);
            decimal probeSize = machine.GetProbeSize(axisIndex);

            string probDist   = machine.ProbeDist.ToString(CultureInfo.InvariantCulture);
            string probDistUp = machine.ProbeDistUp.ToString(CultureInfo.InvariantCulture);
            string probFeed   = machine.ProbeFeed.ToString(CultureInfo.InvariantCulture);

            var result = await Global.Instance.Com.Current.SendCommandAsync("g91 g31 " + axisName + "-" + probDist + " F" + probFeed + " g90", DefaultProbeTimeout);
            if (result?.LastOrDefault()?.ReplyType.HasFlag(EReplyType.ReplyError) == false)
            {
                Global.Instance.Com.Current.QueueCommand("g92 " + axisName + (-probeSize).ToString(CultureInfo.InvariantCulture));
                Global.Instance.Com.Current.QueueCommand("g91 g0" + axisName + probDistUp + " g90");
                return true;
            }

            return false;
        }

        #endregion

        #region EEprom

        public const int DefaultProbeTimeout = 15000;
        public const int DefaultEpromTimeout = 3000;

        public async Task<uint[]> GetEpromValuesAsync(int waitForMilliseconds)
        {
            var cmd = (await Global.Instance.Com.Current.SendCommandAsync("$?", waitForMilliseconds)).FirstOrDefault();
            if (cmd != null && string.IsNullOrEmpty(cmd.ResultText) == false)
            {
                string[] seperators = { "\n", "\r" };
                string[] lines      = cmd.ResultText.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
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

        public async Task WriteEepromValuesAsync(EepromV1 ee)
        {
            await Global.Instance.Com.Current.SendCommandAsync(@"$!", DefaultEpromTimeout);
            await Global.Instance.Com.Current.SendCommandsAsync(ee.ToGCode(), DefaultEpromTimeout);
        }

        public async Task EraseEepromValuesAsync()
        {
            await Global.Instance.Com.Current.SendCommandAsync(@"$!",   DefaultEpromTimeout);
            await Global.Instance.Com.Current.SendCommandAsync(@"$0=0", DefaultEpromTimeout);
        }

        #endregion

        public static string PrepareCommand(string commandString)
        {
            string prefix = GetCommandPrefix();

            if (string.IsNullOrEmpty(prefix))
            {
                return commandString;
            }

            return prefix + commandString;
        }

        protected static string GetCommandPrefix()
        {
            var prefix = Global.Instance.Machine.CommandSyntax;
            if (prefix == CommandSyntax.HPGL)
            {
                return ((char) 27).ToString();
            }

            return null;
        }

        public const int DefaulTimeout = 120 * 1000;

        public async Task SendCommandAsync(string commandString)
        {
            await SendCommandAsync(Global.Instance.Machine, commandString);
        }

        public async Task SendCommandAsync(Machine machine, string commandString)
        {
            string[] seperators = { @"\n" };
            string[] cmds       = commandString.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in cmds)
            {
                string[] infos = s.Split(':');
                int      axis;

                if (infos.Length > 1 && string.Compare(infos[0], @";probe", true) == 0 && -1 != (axis = GCodeHelper.AxisNameToIndex(infos[1])))
                {
                    if (false == await SendProbeCommandAsync(machine, axis))
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
                        var result = await Global.Instance.Com.Current.SendCommandAsync(s.TrimEnd().TrimEnd('?'), DefaulTimeout);
                        if (result?.LastOrDefault()?.ReplyType.HasFlag(EReplyType.ReplyError) == false)
                        {
                            return;
                        }
                    }
                    else
                    {
                        await Global.Instance.Com.Current.SendCommandAsync(s, DefaulTimeout);
                    }
                }
            }
        }
    }
}