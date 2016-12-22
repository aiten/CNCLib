using Framework.Arduino;
using System;
using System.Globalization;
using System.Threading.Tasks;
using CNCLib.GCode;
using CNCLib.Logic.Contracts.DTO;
using System.Media;

namespace CNCLib.Wpf.Helpers
{
    public class MachineGCodeHelper
	{
		public Framework.Arduino.ArduinoSerialCommunication Com
		{
			get { return Framework.Tools.Pattern.Singleton<Framework.Arduino.ArduinoSerialCommunication>.Instance; }
		}
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
			if ((Com.LastCommand.ReplyType & ArduinoSerialCommunication.EReplyType.ReplyError) == 0)
			{
				Com.SendCommand("g92 " + axisname + (-probesize).ToString(CultureInfo.InvariantCulture));
				Com.SendCommand("g91 g0" + axisname + probdistup + " g90");
				return true;
			};
			return false;
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
					await Com.SendCommandAsync(s);
				}
			}
		}
	}
}
