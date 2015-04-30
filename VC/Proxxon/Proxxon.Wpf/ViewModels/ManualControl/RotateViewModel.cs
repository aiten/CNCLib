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
	public class RotateViewModel : DetailViewModel
	{
		public RotateViewModel(IManualControlViewModel vm)
			: base(vm)
		{
		}

		#region Properties


		#endregion

		#region Commands / CanCommands

		public void SendG69()							{ AsyncRunCommand(() => { Com.SendCommand("g69"); }); }
		public void SendG68X0Y0R90()					{ AsyncRunCommand(() => { Com.SendCommand("g68 x0y0r90"); }); }
		public void SendG68X0Y0R270()                   { AsyncRunCommand(() => { Com.SendCommand("g68 x0y0r270"); }); }

		#endregion

		#region ICommand
		public ICommand SendG69Command { get { return new DelegateCommand(SendG69, CanSend); } }
		public ICommand SendG68X0Y0R90Command { get { return new DelegateCommand(SendG68X0Y0R90, CanSend); } }
		public ICommand SendG68X0Y0R270Command { get { return new DelegateCommand(SendG68X0Y0R270, CanSend); } }

		#endregion
	}
}
