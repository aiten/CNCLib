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
	public class CommandHistoryViewModel : DetailViewModel
	{
		public CommandHistoryViewModel(IManualControlViewModel vm)
			: base(vm)
		{
		}

		public const string CommandHistoryFile = @"c:\tmp\Command.txt";

		#region Properties

		private ObservableCollection<ProxxonCommand> _ProxxonCommandCollection;
		public ObservableCollection<ProxxonCommand> ProxxonCommandCollection
		{
			get { return _ProxxonCommandCollection; }
			set { AssignProperty(ref _ProxxonCommandCollection, value); }
		}

		#endregion

		#region Commands / CanCommands

		public void RefreshAfterCommand()
		{
			RefreshCommandHistory();
		}

		public void RefreshCommandHistory()
		{
			lock (this)
			{
				var results = new ObservableCollection<ProxxonCommand>();

				foreach (ArduinoSerialCommunication.Command rc in Com.CommandHistory)
				{
					results.Add(new ProxxonCommand() { CommandDate = rc.SentTime, CommandText = rc.CommandText, Result = rc.ResultText });

				}
				ProxxonCommandCollection = results;
			}
		}
		public void ClearCommandHistory()
		{
			Com.ClearCommandHistory();
			RefreshCommandHistory();
		}

		#endregion

		#region ICommand

		public ICommand RefreshHistoryCommand { get { return new DelegateCommand(RefreshCommandHistory, CanSend); } }
		public ICommand ClearHistoryCommand { get { return new DelegateCommand(ClearCommandHistory, CanSend); } }

		#endregion
	}
}
