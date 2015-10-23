﻿////////////////////////////////////////////////////////
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
	public class DirectCommandViewModel : DetailViewModel
	{
		public DirectCommandViewModel(IManualControlViewModel vm) : base(vm)
		{
		}

		#region Properties

		#endregion

		#region DirectCommand

		private string _directCommand;
		public string DirectCommand
		{
			get { return _directCommand; }
			set { SetProperty(ref _directCommand, value); }
		}

		private void AddDirectCommandHistory(string cmd)
		{
			if (_directCommandHistory == null) _directCommandHistory = new ObservableCollection<string>();
			_directCommandHistory.Add(cmd);
			DirectCommandHistory = _directCommandHistory;
		}

		private ObservableCollection<string> _directCommandHistory;
		public ObservableCollection<string> DirectCommandHistory
		{
			get { return _directCommandHistory; }
			set { AssignProperty(ref _directCommandHistory, value); }
		}

		#endregion

		#region Commands / CanCommands

		public void SendDirect() { AsyncRunCommand(() => { Com.SendCommand(DirectCommand); }); AddDirectCommandHistory(DirectCommand); }
		public bool CanSendDirectCommand()
		{
			return Connected && !string.IsNullOrEmpty(DirectCommand);
		}

		#endregion

		#region ICommand
		public ICommand SendDirectCommand { get { return new DelegateCommand(SendDirect, CanSendDirectCommand); } }

		#endregion
	}
}