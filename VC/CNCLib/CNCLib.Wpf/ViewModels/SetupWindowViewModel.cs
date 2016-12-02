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
using System.Threading.Tasks;

namespace CNCLib.Wpf.ViewModels
{
	public class SetupWindowViewModel : BaseViewModel
    {
		#region crt

		public SetupWindowViewModel()
		{
 			ResetOnConnect = false;
		}

		public override async Task Loaded()
		{
			await base.Loaded();
			await LoadMachines(-1);
			await LoadJoystick();
		}

		#endregion

		#region private operations

		private async Task LoadMachines(int defaultmachineid)
        {
            var machines = new ObservableCollection<Models.Machine>();

			using (var controller = Dependency.Resolve<IMachineService>())
			{
				foreach(var m in await controller.GetAll())
				{
					machines.Add(Converter.Convert(m));
				}
				int defaultM = await controller.GetDetaultMachine();

				Machines = machines;

				var defaultmachine = machines.FirstOrDefault((m) => m.MachineID == defaultmachineid);

				if (defaultmachine == null)
					defaultmachine = machines.FirstOrDefault((m) => m.MachineID == defaultM);

				if (defaultmachine == null && machines.Count > 0)
					defaultmachine = machines[0];

				Machine = defaultmachine;
			}
		}
		private async Task LoadJoystick()
		{
			Joystick = (await JoystickHelper.Load()).Item1;
		}

		#endregion

		#region GUI-forward

		public Action<int> EditMachine { get; set; }
		public Action EditJoystick { get; set; }

		#endregion

		#region Properties

		private Framework.Arduino.ArduinoSerialCommunication Com
        {
			get { return Framework.Tools.Pattern.Singleton<Framework.Arduino.ArduinoSerialCommunication>.Instance; }
        }

        private Framework.Arduino.ArduinoSerialCommunication ComJoystick
        {
            get { return Framework.Tools.Pattern.Singleton<JoystickArduinoSerialCommunication>.Instance; }
        }

        #region Current Machine

        public Models.Machine Machine
		{
            get { return _selectedMachine; }
			set {
                    SetProperty(ref _selectedMachine, value);
                    if (value!=null)
                        SetGlobal();
                }
		}

		public Joystick Joystick { get; set; }

		Models.Machine _selectedMachine;

        private ObservableCollection<Models.Machine> _machines;
        public ObservableCollection<Models.Machine> Machines
		{
			get { return _machines; }
			set { SetProperty(ref _machines, value); }
		}

        public bool Connected
        {
            get { return Com.IsConnected; }
        }

        public bool ConnectedJoystick
        {
            get { return ComJoystick.IsConnected; }
        }

        #endregion

        private bool _resetOnConnect=true;
		public bool ResetOnConnect
		{
			get { return _resetOnConnect; }
			set { SetProperty(ref _resetOnConnect, value); }
		}

		private bool _sendInitCommands = true;
		public bool SendInitCommands
		{
			get { return _sendInitCommands; }
			set { SetProperty(ref _sendInitCommands, value); }
		}

		#endregion

		#region Operations

		public async Task Connect()
        {
			try
			{
				Com.ResetOnConnect = ResetOnConnect;
                Com.CommandToUpper = Machine.CommandToUpper;
                Com.BaudRate = (int)Machine.BaudRate;
                Com.Connect(Machine.ComPort);
                await SetGlobal();

				if (SendInitCommands && Machine != null)
				{
					var initCommands = Machine.MachineInitCommands;

					if (initCommands.Count() > 0)
					{
						// wait (do not check if reset - arduino may reset even the "reset" is not specified)
						await Com.WaitUntilResonseAsync(3000);

						foreach (var initcmd in initCommands.OrderBy(cmd => cmd.SeqNo))
						{
							Com.QueueCommand(initcmd.CommandString);
						}
					}
				}
			}
			catch(Exception e)
			{
				MessageBox?.Invoke("Open serial port failed? " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			OnPropertyChanged(() => Connected);
			CommandManager.InvalidateRequerySuggested();
		}

        public void ConnectJoystick()
        {
            try
            {
                ComJoystick.ResetOnConnect = true;
                ComJoystick.CommandToUpper = false;
                ComJoystick.BaudRate = Joystick.BaudRate;
                ComJoystick.Connect(Joystick.ComPort);
            }
            catch (Exception e)
            {
				MessageBox?.Invoke("Open serial port failed? " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            OnPropertyChanged(() => ConnectedJoystick);
        }


        private async Task SetGlobal()
        {
			Settings.Instance.SizeX = Machine.SizeX;
			Settings.Instance.SizeY = Machine.SizeY;
			Settings.Instance.SizeZ = Machine.SizeZ;
            Com.ArduinoBuffersize = Machine.BufferSize;

			using (var controller = Dependency.Resolve<IMachineService>())
			{
				Global.Instance.Machine = await controller.Get(Machine.MachineID);
			}
		}

		public bool CanConnect()
        {
            return !Connected && Machine != null;
        }

		public void DisConnect()
		{
			Com.Disconnect();
			OnPropertyChanged(() => Connected);
		}
		public bool CanDisConnect()
		{
			return Connected;
		}

        public bool CanConnectJoystick()
        {
            return !ConnectedJoystick;
        }

        public void DisConnectJoystick()
        {
            ComJoystick.Disconnect();
            OnPropertyChanged(() => ConnectedJoystick);
        }
        public bool CanDisConnectJoystick()
        {
            return ConnectedJoystick;
        }


        public async void SetupMachine()
        {
            var mID = Machine!=null ? Machine.MachineID : -1;
			EditMachine?.Invoke(mID);
            await LoadMachines(mID);
        }

		public async void SetupJoystick()
		{
			EditJoystick?.Invoke();
			await LoadJoystick();
		}

		public void SetDefaultMachine()
	   {
            if (Machine != null)
            {
                using (var controller = Dependency.Resolve<IMachineService>())
                {
                    controller.SetDetaultMachine(Machine.MachineID);
                }
            }
	   }

		public bool CanSetupMachine()
        {
            return !Connected;
        }

        public bool CanShowPaint()
        {
            return Connected;
        }
		public bool CanSetupJoystick()
		{
			return !ConnectedJoystick;
		}

		#endregion

		#region Commands

		public ICommand SetupMachineCommand { get { return new DelegateCommand(SetupMachine, CanSetupMachine); } }
		public ICommand ConnectCommand { get { return new DelegateCommand(async () => await Connect(), CanConnect); } }
		public ICommand DisConnectCommand	{ get { return new DelegateCommand(DisConnect, CanDisConnect); } }
		public ICommand SetDefaultMachineCommand { get { return new DelegateCommand(SetDefaultMachine, CanSetupMachine); } }
        public ICommand ConnectJoystickCommand { get { return new DelegateCommand(ConnectJoystick, CanConnectJoystick); } }
        public ICommand DisConnectJoystickCommand { get { return new DelegateCommand(DisConnectJoystick, CanDisConnectJoystick); } }
		public ICommand SetupJoystickCommand { get { return new DelegateCommand(SetupJoystick, CanSetupJoystick); } }

		#endregion
	}
}
