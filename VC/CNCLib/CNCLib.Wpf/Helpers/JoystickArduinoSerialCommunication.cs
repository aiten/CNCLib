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
				if (string.Compare(info.Info.Trim(),";g31:z", true) == 0)
				{
					new MachineGCodeHelper().SendProbeCommandAsync(2).GetAwaiter().GetResult();
				}
				else
				{
					Com.SendCommand(info.Info);
				}
			})
			;
        }
    }
}
