////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;
using Proxxon.Wpf.Models;
using System.Threading;
using System.IO;
using Framework.Logic;
using System.Globalization;

namespace Proxxon.Wpf.ViewModels.ManualControl
{
	public class ToolViewModel : DetailViewModel
	{
		public ToolViewModel(IManualControlViewModel vm)
			: base(vm)
		{
		}

		#region Properties


		#endregion

		#region Commands / CanCommands

		public void SendInfo() { AsyncRunCommand(() => { Com.SendCommand("?"); }); }
		public void SendAbort() { AsyncRunCommand(() => { Com.AbortCommands(); Com.ResumAfterAbort(); Com.SendCommand("!"); }); }
		public void SendProxxonCommand(string command) { AsyncRunCommand(() => { Com.SendCommand(command); }); }
		public void SendM03SpindelOn() { AsyncRunCommand(() => { Com.SendCommand("m3"); }); }
		public void SendM05SpindelOff() { AsyncRunCommand(() => { Com.SendCommand("m5"); }); }
		public void SendM07CoolandOn() { AsyncRunCommand(() => { Com.SendCommand("m7"); }); }
		public void SendM09CoolandOff() { AsyncRunCommand(() => { Com.SendCommand("m9"); }); }
		public void SendM114PrintPos()
		{
			AsyncRunCommand(() =>
			{
				string message = Com.SendCommandAndRead("m114");

				if (!string.IsNullOrEmpty(message))
				{
					message = message.Replace("ok", "");
					message = message.Replace(" ", "");
					SetPositions(message.Split(':'));
				}
			});
		}


		#endregion

		#region ICommand
		public ICommand SendInfoCommand { get { return new DelegateCommand(SendInfo, CanSend); } }
		public ICommand SendAbortCommand { get { return new DelegateCommand(SendAbort, CanSend); } }
		public ICommand SendM03SpindelOnCommand { get { return new DelegateCommand(SendM03SpindelOn, CanSend); } }
		public ICommand SendM05SpindelOffCommand { get { return new DelegateCommand(SendM05SpindelOff, CanSend); } }
		public ICommand SendM07CoolandOnCommand { get { return new DelegateCommand(SendM07CoolandOn, CanSend); } }
		public ICommand SendM09CoolandOffCommand { get { return new DelegateCommand(SendM09CoolandOff, CanSend); } }
		public ICommand SendM114Command { get { return new DelegateCommand(SendM114PrintPos, CanSend); } }

		#endregion
	}
}
