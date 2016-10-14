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
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CNCLib.GCode;
using CNCLib.GCode.Commands;
using CNCLib.Logic.Contracts.DTO;
using Framework.Wpf.Helpers;
using Framework.Wpf.ViewModels;

namespace CNCLib.Wpf.ViewModels
{
	public class PreviewViewModel : BaseViewModel
	{
		#region crt

		public PreviewViewModel()
		{
		}

		#endregion

		#region Properties

		public Framework.Arduino.ArduinoSerialCommunication Com
		{
			get { return Framework.Tools.Pattern.Singleton<Framework.Arduino.ArduinoSerialCommunication>.Instance; }
		}

		private CommandList _commands = new CommandList();

		public CommandList Commands
		{
			get { return _commands; }
			set { SetProperty(() => _commands == value, () => _commands = value); }
		}

		#endregion

		#region GUI-forward

		public class GetLoadInfoArg
		{
			public LoadOptions LoadOption { get; set; }
			public bool UseAzure { get; set; }
		}
		public Func<GetLoadInfoArg, bool> GetLoadInfo { get; set; }

		#endregion

		#region private/intern

		LoadOptions loadinfo = new LoadOptions();
		bool _useAzure = false;
		bool _loadingOrSending = false;

		#endregion

		#region Operations

		public void SendTo()
		{
			new Thread(() =>
			{
				_loadingOrSending = true;

				try
				{
					Com.ClearCommandHistory();
					Com.SendCommandsAsync(Commands.ToStringList()).Wait();
					Com.WriteCommandHistory(@"c:\tmp\Command.txt");
				}
				finally
				{
					_loadingOrSending = false;
				}
			}
			).Start();

		}

		public async void Load()
		{
			if (loadinfo.AutoScaleSizeX == 0 || loadinfo.AutoScale == false)
			{
				loadinfo.AutoScaleSizeX = Settings.Instance.SizeX;
				loadinfo.AutoScaleSizeY = Settings.Instance.SizeY;
			}

			var arg = new GetLoadInfoArg() { LoadOption = loadinfo, UseAzure = _useAzure };
			if ((GetLoadInfo?.Invoke(arg)).GetValueOrDefault(false))
			{
				loadinfo = arg.LoadOption;
				_useAzure = arg.UseAzure;

				var ld = new GCodeLoad();

				try
				{
					_loadingOrSending = true;
					Commands = await ld.Load(loadinfo, _useAzure);
				}
				catch (Exception ex)
				{
					MessageBox?.Invoke("Load failed with error: " + ex.Message, "CNCLib", MessageBoxButton.OK, MessageBoxImage.Stop);
				}
				finally
				{
					_loadingOrSending = false;
				}
			}
		}

		public bool CanSendTo()
		{
//			return !_loading;
			return !_loadingOrSending && Com.IsConnected;
		}

		public bool CanLoad()
		{
			return _loadingOrSending == false;
		}

		#endregion

		#region Commands

		public ICommand LoadCommand { get { return new DelegateCommand(Load, CanLoad); } }
		public ICommand SendToCommand { get { return new DelegateCommand(SendTo, CanSendTo); } }

		#endregion
	}
}
