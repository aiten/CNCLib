using Framework.Arduino;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			string axisname = Global.Instance.Machine.GetAxisName(axisindex);
			decimal probesize = Global.Instance.Machine.GetProbeSize(axisindex);

			string probdist = Global.Instance.Machine.ProbeDist.ToString(CultureInfo.InvariantCulture);
			string probdistup = Global.Instance.Machine.ProbeDistUp.ToString(CultureInfo.InvariantCulture);
			string probfeed = Global.Instance.Machine.ProbeFeed.ToString(CultureInfo.InvariantCulture);

			await Com.SendCommandAndReadOKReplyAsync("g91 g31 " + axisname + "-" + probdist + " F" + probfeed + " g90");
			if ((Com.LastCommand.ReplyType & ArduinoSerialCommunication.EReplyType.ReplyError) == 0)
			{
				Com.SendCommand("g92 " + axisname + (-probesize).ToString(CultureInfo.InvariantCulture));
				Com.SendCommand("g91 g0" + axisname + probdistup + " g90");
				return true;
			};
			return false;
		}
	}
}
