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

using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;
using System.Windows;
using Framework.Tools;
using CNCLib.GCode;
using CNCLib.Wpf.Models;
using CNCLib.Logic.Contracts;
using CNCLib.ServiceProxy;
using Framework.Tools.Dependency;
using CNCLib.Wpf.Helpers;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.GUI.Load;
using CNCLib.GCode.Commands;

namespace CNCLib.Wpf.ViewModels
{
	public class PreviewViewModel : BaseViewModel
	{
		public PreviewViewModel()
		{
		}

		#region Properties

		private Framework.Arduino.ArduinoSerialCommunication Com
		{
			get { return Framework.Tools.Pattern.Singleton<Framework.Arduino.ArduinoSerialCommunication>.Instance; }
		}

		public Action<CommandList> GCodeLoaded { get; set; }

		public class GetLoadInfoArg
		{
			public LoadOptions LoadOption { get; set; }
			public bool UseAzure { get; set; }
		}
		public Func<GetLoadInfoArg, bool> GetLoadInfo { get; set; }

		#endregion

		LoadOptions loadinfo = new LoadOptions();
		bool _useAzure = false;
		bool _loading = false;

		#region Operations

		public void SendTo()
		{
//			var commands = _gCodeCtrl.Commands.ToStringList();
//			Com.SendCommands(commands);
		}

		public async void Load()
		{
			if (loadinfo.AutoScaleSizeX == 0 || loadinfo.AutoScale == false)
			{
				loadinfo.AutoScaleSizeX = Settings.Instance.SizeX;
				loadinfo.AutoScaleSizeY = Settings.Instance.SizeY;
			}

			var arg = new GetLoadInfoArg() { LoadOption = loadinfo, UseAzure = _useAzure };

			if (GetLoadInfo != null && GetLoadInfo(arg))
			{
				loadinfo = arg.LoadOption;
				_useAzure = arg.UseAzure;

				var ld = new GCodeLoad();

				try
				{
					_loading = true;
					CommandList commands = await ld.Load(loadinfo, _useAzure);
					GCodeLoaded?.Invoke(commands);
				}
				catch (Exception ex)
				{
					throw;
				}
				finally
				{
					_loading = false;
				}
			}
		}

		public bool CanSendTo()
		{
			return Com.IsConnected;
		}

		public bool CanLoad()
		{
			return _loading == false;
		}

		#endregion

		#region Commands

		public ICommand LoadCommand { get { return new DelegateCommand(Load, CanLoad); } }
		public ICommand SendToCommand { get { return new DelegateCommand(SendTo, CanSendTo); } }

		#endregion
	}
}
