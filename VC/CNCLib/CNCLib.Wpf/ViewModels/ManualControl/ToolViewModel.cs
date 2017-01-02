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
using System.Windows.Input;
using Framework.Wpf.Helpers;
using Framework.Arduino;

namespace CNCLib.Wpf.ViewModels.ManualControl
{
	public class ToolViewModel : DetailViewModel , IDisposable
	{
		#region ctr

		public ToolViewModel(IManualControlViewModel vm)
			: base(vm)
		{
			Com.CommandQueueChanged += new ArduinoSerialCommunication.CommandEventHandler(CommandQueueChanged);
		}

		public void Dispose()
		{
			Com.CommandQueueChanged -= new ArduinoSerialCommunication.CommandEventHandler(CommandQueueChanged);
		}

		#endregion

		#region Properties

		public int PendingCommandCount
		{
			get	{ return Com.CommandsInQueue; }
		}

		private void CommandQueueChanged(object sender, ArduinoSerialCommunicationEventArgs arg)
		{
			OnPropertyChanged(() => PendingCommandCount);
		}

		#endregion

		#region Commands / CanCommands
		public bool CanSendSpindle()
		{
			return CanSend() && Global.Instance.Machine.Spindle;
		}
		public bool CanSendCoolant()
		{
			return CanSend() && Global.Instance.Machine.Coolant;
		}
        public bool CanSendLaser()
        {
            return CanSend() && Global.Instance.Machine.Laser;
        }

        public void SendInfo() { RunInNewTask(() => { Com.SendCommand("?"); }); }
		public void SendAbort() { RunInNewTask(() => { Com.AbortCommands(); Com.ResumeAfterAbort(); Com.SendCommand("!"); }); }
		public void SendResurrect() { RunInNewTask(() => { Com.AbortCommands(); Com.ResumeAfterAbort(); Com.SendCommand("!!!"); }); }
		public void ClearQueue() { RunInNewTask(() => { Com.AbortCommands(); Com.ResumeAfterAbort(); }); }
		public void SendCNCLibCommand(string command) { RunInNewTask(() => { Com.SendCommand(command); }); }
		public void SendM03SpindelOn() { RunInNewTask(() => { Com.SendCommand("m3"); }); }
		public void SendM05SpindelOff() { RunInNewTask(() => { Com.SendCommand("m5"); }); }
		public void SendM07CoolandOn() { RunInNewTask(() => { Com.SendCommand("m7"); }); }
		public void SendM09CoolandOff() { RunInNewTask(() => { Com.SendCommand("m9"); }); }
        public void SendM106LaserOn() { RunInNewTask(() => { Com.SendCommand("m106 s255"); }); }
        public void SendM106LaserOnMin() { RunInNewTask(() => { Com.SendCommand("m106 s1"); }); }
        public void SendM107LaserOff() { RunInNewTask(() => { Com.SendCommand("m107"); }); }
        public void SendM114PrintPos()
		{
			RunInNewTask(() =>
			{
				string message = Com.SendCommandAndReadOKReplyAsync("m114").ConfigureAwait(false).GetAwaiter().GetResult();

				if (!string.IsNullOrEmpty(message))
				{
					message = message.Replace("ok", "");
					message = message.Replace(" ", "");
					SetPositions(message.Split(':'),0);
				}

				message = Com.SendCommandAndReadOKReplyAsync("m114 s1").ConfigureAwait(false).GetAwaiter().GetResult();

				if (!string.IsNullOrEmpty(message))
				{
					message = message.Replace("ok", "");
					message = message.Replace(" ", "");
					SetPositions(message.Split(':'),1);
				}
			});
		}
		public void WritePending() { RunInNewTask(() => { Com.WritePendingCommandsToFile(System.IO.Path.GetTempPath() + "PendingCommands.nc"); }); }

		#endregion

		#region ICommand
		public ICommand SendInfoCommand => new DelegateCommand(SendInfo, CanSend);
		public ICommand SendAbortCommand => new DelegateCommand(SendAbort, CanSend);
		public ICommand SendResurrectCommand => new DelegateCommand(SendResurrect, CanSend);
		public ICommand SendClearQueue => new DelegateCommand(ClearQueue, CanSend);
		public ICommand SendM03SpindelOnCommand => new DelegateCommand(SendM03SpindelOn, CanSendSpindle);
		public ICommand SendM05SpindelOffCommand => new DelegateCommand(SendM05SpindelOff, CanSendSpindle);
		public ICommand SendM07CoolandOnCommand => new DelegateCommand(SendM07CoolandOn, CanSendCoolant);
		public ICommand SendM09CoolandOffCommand => new DelegateCommand(SendM09CoolandOff, CanSendCoolant);
        public ICommand SendM106LaserOnCommand => new DelegateCommand(SendM106LaserOn, CanSendLaser);
        public ICommand SendM106LaserOnMinCommand => new DelegateCommand(SendM106LaserOnMin, CanSendLaser);
        public ICommand SendM107LaserOffCommand => new DelegateCommand(SendM107LaserOff, CanSendLaser);
        public ICommand SendM114Command => new DelegateCommand(SendM114PrintPos, CanSend);
		public ICommand WritePendingCommands => new DelegateCommand(WritePending, CanSend);

		#endregion
	}
}
