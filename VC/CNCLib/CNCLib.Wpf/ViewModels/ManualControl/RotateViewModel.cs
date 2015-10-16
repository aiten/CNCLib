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
using CNCLib.Wpf.Models;
using System.Threading;
using System.IO;
using Framework.Logic;
using System.Globalization;

namespace CNCLib.Wpf.ViewModels.ManualControl
{
	public class RotateViewModel : DetailViewModel
	{
		public RotateViewModel(IManualControlViewModel vm)
			: base(vm)
		{
		}

		#region Properties


		#endregion

		#region Commands / CanCommands

		public bool CanSendRotate()
		{
			return CanSend() && Global.Instance.Machine.Rotate;
		}

		public void SendG69()							{ AsyncRunCommand(() => { Com.SendCommand("g69"); }); }
		public void SendG68X0Y0R90()					{ AsyncRunCommand(() => { Com.SendCommand("g68 x0y0r90"); }); }
		public void SendG68X0Y0R270()                   { AsyncRunCommand(() => { Com.SendCommand("g68 x0y0r270"); }); }
		public void SendG6810()							{ AsyncRunCommand(() => { Com.SendCommand("g68.10"); }); }
		public void SendG6811()							{ AsyncRunCommand(() => { Com.SendCommand("g68.11"); }); }
		public void SendG6813()							{ AsyncRunCommand(() => { Com.SendCommand("g68.13 j0k0"); }); }
		public void SendG6814()							{ AsyncRunCommand(() => { Com.SendCommand("g68.14 i0"); }); }

		#endregion

		#region ICommand
		public ICommand SendG69Command			{ get { return new DelegateCommand(SendG69, CanSendRotate); } }
		public ICommand SendG68X0Y0R90Command	{ get { return new DelegateCommand(SendG68X0Y0R90, CanSendRotate); } }
		public ICommand SendG68X0Y0R270Command	{ get { return new DelegateCommand(SendG68X0Y0R270, CanSendRotate); } }

		public ICommand SendG6810Command		{ get { return new DelegateCommand(SendG6810, CanSendRotate); } }
		public ICommand SendG6811Command		{ get { return new DelegateCommand(SendG6811, CanSendRotate); } }
		public ICommand SendG6813Command		{ get { return new DelegateCommand(SendG6813, CanSendRotate); } }
		public ICommand SendG6814Command		{ get { return new DelegateCommand(SendG6814, CanSendRotate); } }


		#endregion
	}
}
