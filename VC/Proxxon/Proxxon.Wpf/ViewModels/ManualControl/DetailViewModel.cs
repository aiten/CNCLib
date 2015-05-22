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
	public class DetailViewModel : BaseViewModel
	{
		private IManualControlViewModel Vm { get; set; }
		public DetailViewModel(IManualControlViewModel vm)
		{
			Vm = vm;
		}
		public Framework.Logic.ArduinoSerialCommunication Com
		{
			get { return Framework.Tools.Singleton<Framework.Logic.ArduinoSerialCommunication>.Instance; }
		}
		public bool Connected
		{
			get { return Com.IsConnected; }
		}
		protected void AsyncRunCommand(Action todo)
		{
			Vm.AsyncRunCommand(todo);
		}
		protected void SetPositions(string[] positions, int positionIdx)
		{
			Vm.SetPositions(positions, positionIdx);
		}

		#region Command/CanCommand

		public bool CanSend()
		{
			return Connected;
		}

		#endregion
	}
}
