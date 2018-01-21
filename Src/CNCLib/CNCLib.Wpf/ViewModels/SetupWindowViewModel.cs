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

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CNCLib.ServiceProxy;
using CNCLib.Wpf.Helpers;
using CNCLib.Wpf.Models;
using Framework.Wpf.Helpers;
using Framework.Wpf.ViewModels;

namespace CNCLib.Wpf.ViewModels
{
    public class SetupWindowViewModel : BaseViewModel
    {
		#region crt

		public SetupWindowViewModel(IMachineService machineService)
		{
            _machineService = machineService ?? throw new ArgumentNullException(); 
             ResetOnConnect = false;
		}

        readonly IMachineService _machineService;


        public override async Task Loaded()
		{
			await base.Loaded();
			if (_machines == null)
			{
				await LoadMachines(-1);
				await LoadJoystick();
			}
		}

		#endregion

		#region private operations

		private async Task LoadMachines(int defaultmachineid)
        {
            var machines = new ObservableCollection<Machine>();

			foreach(var m in await _machineService.GetAll())
			{
				machines.Add(Converter.Convert(m));
			}
			int defaultM = await _machineService.GetDetaultMachine();

			Machines = machines;

			var defaultmachine = machines.FirstOrDefault(m => m.MachineID == defaultmachineid);

			if (defaultmachine == null)
				defaultmachine = machines.FirstOrDefault(m => m.MachineID == defaultM);

			if (defaultmachine == null && machines.Count > 0)
				defaultmachine = machines[0];

			Machine = defaultmachine;
		}
		private async Task LoadJoystick()
		{
			Joystick = (await JoystickHelper.Load()).Item1;
			Global.Instance.Joystick = Joystick;
		}

		#endregion

		#region GUI-forward

		public Action<int> EditMachine { get; set; }
		public Action EditJoystick { get; set; }
		public Action ShowEeprom { get; set; }

		#endregion

		#region Properties

        #region Current Machine

        public Machine Machine
		{
            get => _selectedMachine;
            set
            {
                SetProperty(ref _selectedMachine, value);
                if (value != null)
                {
                    RaisePropertyChanged(nameof(NeedDtr));
                    SetGlobal();
                }
            }
		}

		public Joystick Joystick { get; set; }

		Machine _selectedMachine;

        private ObservableCollection<Machine> _machines;
        public ObservableCollection<Machine> Machines
		{
			get => _machines;
            set => SetProperty(ref _machines, value);
        }

        public bool Connected => Global.Instance.Com.IsConnected;

        public bool ConnectedJoystick => Global.Instance.ComJoystick.IsConnected;

        #endregion

        private bool _resetOnConnect=true;
		public bool ResetOnConnect
		{
			get => _resetOnConnect;
		    set { SetProperty(ref _resetOnConnect, value); Global.Instance.ResetOnConnect = value; }
		}

		private bool _sendInitCommands = true;
		public bool SendInitCommands
		{
			get => _sendInitCommands;
		    set => SetProperty(ref _sendInitCommands, value);
		}

        public bool NeedDtr => Machine != null && Machine.NeedDtr;

        #endregion

        #region Operations

        public async Task Connect()
        {
			try
			{
                if (Machine.NeedDtr)
                    Global.Instance.Com.ResetOnConnect = true;
                else
                    Global.Instance.Com.ResetOnConnect = ResetOnConnect;
                Global.Instance.Com.CommandToUpper = Machine.CommandToUpper;
                Global.Instance.Com.BaudRate = (int)Machine.BaudRate;
                Global.Instance.Com.Connect(Machine.ComPort);
                await SetGlobal();

				if (SendInitCommands && Machine != null)
				{
					var initCommands = Machine.MachineInitCommands;

					if (initCommands.Any())
					{
						// wait (do not check if reset - arduino may reset even the "reset" is not specified)
						await Global.Instance.Com.WaitUntilResponseAsync(3000);

						foreach (var initcmd in initCommands.OrderBy(cmd => cmd.SeqNo))
						{
							Global.Instance.Com.QueueCommand(initcmd.CommandString);
						}
					}
				}
			}
			catch(Exception e)
			{
				MessageBox?.Invoke("Open serial port failed? " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			RaisePropertyChanged(nameof(Connected));
			CommandManager.InvalidateRequerySuggested();
		}

        public void ConnectJoystick()
        {
            try
            {
                Global.Instance.ComJoystick.ResetOnConnect = true;
                Global.Instance.ComJoystick.CommandToUpper = false;
                Global.Instance.ComJoystick.BaudRate = Joystick.BaudRate;
                Global.Instance.ComJoystick.Connect(Joystick.ComPort);
            }
            catch (Exception e)
            {
				MessageBox?.Invoke("Open serial port failed? " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            RaisePropertyChanged(nameof(ConnectedJoystick));
        }

        private async Task SetGlobal()
        {
			Global.Instance.SizeX = Machine.SizeX;
            Global.Instance.SizeY = Machine.SizeY;
            Global.Instance.SizeZ = Machine.SizeZ;
			Global.Instance.Com.ArduinoBuffersize = Machine.BufferSize;

			Global.Instance.Machine = await _machineService.Get(Machine.MachineID);
        }

		public bool CanConnect()
        {
            return !Connected && Machine != null;
        }

		public void DisConnect()
		{
			Global.Instance.Com.Disconnect();
			RaisePropertyChanged(nameof(Connected));
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
            Global.Instance.ComJoystick.Disconnect();
            RaisePropertyChanged(nameof(ConnectedJoystick));
        }
        public bool CanDisConnectJoystick()
        {
            return ConnectedJoystick;
        }

        public async void SetupMachine()
        {
            int mID = Machine?.MachineID ?? -1;
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
                _machineService.SetDetaultMachine(Machine.MachineID);
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

		public void SetEeprom()
		{
			ShowEeprom?.Invoke();
		}

		#endregion

		#region Commands

		public ICommand SetupMachineCommand => new DelegateCommand(SetupMachine, CanSetupMachine);
		public ICommand ConnectCommand => new DelegateCommand(async () => await Connect(), CanConnect);
		public ICommand DisConnectCommand	=> new DelegateCommand(DisConnect, CanDisConnect);
		public ICommand EepromCommand => new DelegateCommand(SetEeprom, CanDisConnect);
		public ICommand SetDefaultMachineCommand => new DelegateCommand(SetDefaultMachine, CanSetupMachine);
        public ICommand ConnectJoystickCommand => new DelegateCommand(ConnectJoystick, CanConnectJoystick);
        public ICommand DisConnectJoystickCommand => new DelegateCommand(DisConnectJoystick, CanDisConnectJoystick);
		public ICommand SetupJoystickCommand => new DelegateCommand(SetupJoystick, CanSetupJoystick);

		#endregion
	}
}
