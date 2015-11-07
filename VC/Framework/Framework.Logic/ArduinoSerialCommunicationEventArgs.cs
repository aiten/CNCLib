using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Logic
{
	public class ArduinoSerialCommunicationEventArgs : EventArgs
	{
		public ArduinoSerialCommunicationEventArgs(string info, ArduinoSerialCommunication.Command cmd)
		{
			Command = cmd;
			if (cmd != null && string.IsNullOrEmpty(info))
				this.Info = cmd.CommandText;
			else
				this.Info = info;
			Continue = false;
			Abort = false;
		}

		public bool Continue { get; set; }
		public bool Abort { get; set; }
		public string Result { get; set; }

		public readonly string Info;

		public ArduinoSerialCommunication.Command Command { get; private set; }
	}
}
