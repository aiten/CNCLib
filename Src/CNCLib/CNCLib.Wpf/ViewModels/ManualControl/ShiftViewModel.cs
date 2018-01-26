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

		public void SendG92() { RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("g92"); }); }
			
		#endregion

		#region ICommand
		public ICommand SendG92Command => new DelegateCommand(SendG92, CanSendGCode);	   

		#endregion
	}
}
