////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
using CNCLib.GCode;
using CNCLib.Logic.Contracts.DTO;
using System.Media;
using System.Collections.Generic;

namespace CNCLib.Wpf.Helpers
{
    public class MachineGCodeHelper
	{
		public Framework.Arduino.SerialCommunication.ISerial Com => Framework.Tools.Pattern.Singleton<Framework.Arduino.SerialCommunication.Serial>.Instance;

	    #region Probe

		public async Task<bool> SendProbeCommandAsync(int axisindex)
		{
			return await SendProbeCommandAsync(Global.Instance.Machine, axisindex);
		}

		public async Task<bool> SendProbeCommandAsync(Machine machine, int axisindex)
		{
			string axisname = machine.GetAxisName(axisindex);
			decimal probesize = machine.GetProbeSize(axisindex);

			string probdist = machine.ProbeDist.ToString(CultureInfo.InvariantCulture);
			string probdistup = machine.ProbeDistUp.ToString(CultureInfo.InvariantCulture);
			string probfeed = machine.ProbeFeed.ToString(CultureInfo.InvariantCulture);

			await Com.SendCommandAndReadOKReplyAsync("g91 g31 " + axisname + "-" + probdist + " F" + probfeed + " g90");
			if ((Com.LastCommand.ReplyType & Framework.Arduino.SerialCommunication.EReplyType.ReplyError) == 0)
			{
				Com.QueueCommand("g92 " + axisname + (-probesize).ToString(CultureInfo.InvariantCulture));
				Com.QueueCommand("g91 g0" + axisname + probdistup + " g90");
				return true;
			};
			return false;
		}

		#endregion

		#region EEprom

		public async Task<UInt32[]> GetEpromValuesAsync(int waitForMilliseconds = 3000)
		{
			var cmd = (await Com.SendCommandAsync("$?", waitForMilliseconds)).FirstOrDefault();
			if (cmd != null && string.IsNullOrEmpty(cmd.ResultText)==false)
			{
				string[] seperators = { "\n", "\r" };
				string[] lines = cmd.ResultText.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
				var intvalues = new Dictionary<int, uint>();
				int maxslot = -1;
				foreach (var line in lines)
				{
					// e.g. $1=65535(ffff)
					string[] assign = line.Split('=');

					int slot;
					if (assign.Length == 2 && assign[0].StartsWith("$") && int.TryParse(assign[0].TrimStart('$'), out slot))
					{
						uint slotvalue;
						string valuestr = assign[1];
						int idx1 = valuestr.IndexOf('(');
						if (idx1 > 0)
							valuestr = valuestr.Substring(0,idx1);

						if (UInt32.TryParse(valuestr, out slotvalue))
						{
							intvalues[slot] = slotvalue;
							if (maxslot < slot)
								maxslot = slot;
						}
					}
				}
				if (maxslot > 0)
				{
					UInt32[] ret = new UInt32[maxslot + 1];
					for (int i=0;i<= maxslot;i++)
					{
						if (intvalues.ContainsKey(i))
							ret[i] = intvalues[i];
					}
					return ret;
				}
			}
			return null;
		}

		public async Task WriteEepromValuesAsync(EepromV1 ee)
		{
			await Com.SendCommandAsync(@"$!");
			await Com.SendCommandsAsync(ee.ToGCode());
		}

		public async Task EraseEepromValuesAsync()
		{
			await Com.SendCommandAsync(@"$!");
			await Com.SendCommandAsync(@"$0=0");
		}

		#endregion

        public static string PrepareCommand(string commandstring)
        {
            var prefix = GetCommandPrefix();

            if (string.IsNullOrEmpty(prefix))
                return commandstring;

            return prefix + commandstring;
        }

        protected static string GetCommandPrefix()
        {
            var prefix = Global.Instance.Machine.CommandSyntax;
            if (prefix == CommandSyntax.HPGL)
                return ((char) 27).ToString();

            return null;
        }

        public async Task SendCommandAsync(string commandstring)
		{
			await SendCommandAsync(Global.Instance.Machine, commandstring);
		}

		public async Task SendCommandAsync(Machine machine, string commandstring)
		{
			string[] seperators = { @"\n" };
			string[] cmds = commandstring.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
			foreach (var s in cmds)
			{
				string[] infos = s.Split(':');
				int axis;

				if (infos.Length > 1 && string.Compare(infos[0], ";probe", true) == 0 && -1 != (axis = GCodeHelper.AxisNameToIndex(infos[1])))
				{
					if (false == await SendProbeCommandAsync(machine, axis))
						break;
				}
				else if (infos.Length == 1 && string.Compare(infos[0], ";beep", true) == 0)
				{
					SystemSounds.Beep.Play();
				}
				else
				{
                    if (s.TrimEnd().EndsWith("?"))
                    {
                        await Com.SendCommandAndReadOKReplyAsync(s.TrimEnd().TrimEnd('?'));
                        if ((Com.LastCommand.ReplyType & Framework.Arduino.SerialCommunication.EReplyType.ReplyError) != 0)
                        {
                            return;
                        }
                    }
                    else
                    {
                        await Com.SendCommandAsync(s);
                    }
                }
			}
		}
	}
}
