////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

using CNCLib.Wpf.ViewModels.ManualControl;
using Framework.Arduino;
using System;
using System.Threading;
using System.Threading.Tasks;
using CNCLib.GCode;
using System.Linq;

namespace CNCLib.Wpf.Helpers
{
	class JoystickArduinoSerialCommunication : ArduinoSerialCommunication
    {
        private Framework.Arduino.ArduinoSerialCommunication Com
        {
            get { return Framework.Tools.Pattern.Singleton<Framework.Arduino.ArduinoSerialCommunication>.Instance; }
        }

        public void RunCommandInNewTask(Action todo)
        {
            new Task(() =>
            {
                try
                {
                    todo();
                    Com.WriteCommandHistory(CommandHistoryViewModel.CommandHistoryFile);
                }
                finally
                {
                }
            }
            ).Start();
        }

        protected override void OnReplyReceived(ArduinoSerialCommunicationEventArgs info)
        {
            base.OnReplyReceived(info);

            RunCommandInNewTask(() =>
			{
				string trim = info.Info.Trim();

				// ;btn5		=> look for ;btn5
				// ;btn5:x		=> x is presscount - always incremented, look for max x in setting => modulo 

				int idx;

				if ((idx=trim.IndexOf(':')) < 0)
				{
					var mc = Global.Instance.Machine.MachineCommands.Where((m) => m.JoystickMessage == trim).FirstOrDefault();
					if (mc != null)
						trim = mc.CommandString;
				}
				else
				{
					string btn = trim.Substring(0, idx+1);
					var mclist = Global.Instance.Machine.MachineCommands.Where((m) => m.JoystickMessage?.Length > idx && m.JoystickMessage.Substring(0, idx+1) == btn).ToList();

					uint max = 0;
					foreach(var m in mclist)
					{
						uint val = 0;
						if (uint.TryParse(m.JoystickMessage.Substring(idx + 1), out val))
						{
							if (val > max)
								max = val;
						}
					}

					string findcmd = $"{btn}{uint.Parse(trim.Substring(idx + 1)) % (max+1)}";

					var mc = Global.Instance.Machine.MachineCommands.Where((m) => m.JoystickMessage == findcmd).FirstOrDefault();
					if (mc == null)
					{
						// try to find ;btn3 (without :)  
						findcmd = trim.Substring(0, idx);
						mc = Global.Instance.Machine.MachineCommands.Where((m) => m.JoystickMessage == findcmd).FirstOrDefault();
					}

					if (mc != null)
						trim = mc.CommandString;
				}

				new MachineGCodeHelper().SendCommandAsync(trim).ConfigureAwait(false).GetAwaiter().GetResult();
			});
        }
    }
}
