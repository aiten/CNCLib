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

using System.Windows.Input;
using Framework.Wpf.Helpers;

namespace CNCLib.Wpf.ViewModels.ManualControl
{
	public class ShiftViewModel : DetailViewModel
	{
		public ShiftViewModel(IManualControlViewModel vm)
			: base(vm)
		{
		}

		#region Properties


		#endregion

		#region Commands / CanCommands

		public void SendG53() { RunAndUpdate(() => { Com.QueueCommand("g53"); }); }
		public void SendG54() { RunAndUpdate(() => { Com.QueueCommand("g54"); }); }
		public void SendG55() { RunAndUpdate(() => { Com.QueueCommand("g55"); }); }
		public void SendG56() { RunAndUpdate(() => { Com.QueueCommand("g56"); }); }
		public void SendG57() { RunAndUpdate(() => { Com.QueueCommand("g57"); }); }
		public void SendG58() { RunAndUpdate(() => { Com.QueueCommand("g58"); }); }
		public void SendG59() { RunAndUpdate(() => { Com.QueueCommand("g59"); }); }
		public void SendG92() { RunAndUpdate(() => { Com.QueueCommand("g92"); }); }
			
		#endregion

		#region ICommand
		public ICommand SendG53Command => new DelegateCommand(SendG53, CanSendPlotter);
		public ICommand SendG54Command => new DelegateCommand(SendG54, CanSendPlotter);
		public ICommand SendG55Command => new DelegateCommand(SendG55, CanSendPlotter);
		public ICommand SendG56Command => new DelegateCommand(SendG56, CanSendPlotter);
		public ICommand SendG57Command => new DelegateCommand(SendG57, CanSendPlotter);
		public ICommand SendG58Command => new DelegateCommand(SendG58, CanSendPlotter);
		public ICommand SendG59Command => new DelegateCommand(SendG59, CanSendPlotter);
		public ICommand SendG92Command => new DelegateCommand(SendG92, CanSendPlotter);	   

		#endregion
	}
}
