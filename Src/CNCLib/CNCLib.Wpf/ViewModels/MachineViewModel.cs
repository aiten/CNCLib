////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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
using System.Windows.Input;
using System.Collections.ObjectModel;
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;
using CNCLib.Wpf.Models;
using Framework.Tools.Dependency;
using CNCLib.ServiceProxy;
using System.Threading.Tasks;
using System.Windows;
using CNCLib.Wpf.Helpers;
using CNCLib.GCode;

namespace CNCLib.Wpf.ViewModels
{
    public class MachineViewModel : BaseViewModel, IDisposable
	{
		#region crt

		public MachineViewModel(IMachineService machineService)
		{
            _machineService = machineService ?? throw new ArgumentNullException();
            AddNewMachine = false;
		}

        readonly IMachineService _machineService;

        #endregion

        #region dispose

        public void Dispose()
		{
		}

		#endregion

		#region Properties
		private Framework.Arduino.SerialCommunication.ISerial Com => Framework.Tools.Pattern.Singleton<Framework.Arduino.SerialCommunication.Serial>.Instance;

	    Models.Machine _currentMachine = new Models.Machine();

		public Models.Machine Machine
		{
			get => _currentMachine;
		    set { SetProperty(() => _currentMachine == value, () => _currentMachine = value); }
		}

		public ObservableCollection<Models.MachineCommand> MachineCommands => _currentMachine.MachineCommands;


	    public ObservableCollection<Models.MachineInitCommand> MachineInitCommands => _currentMachine.MachineInitCommands;

	    public bool AddNewMachine { get; set; }

		#endregion

		#region Operations

		public async Task LoadMachine(int machineID)
		{
			CNCLib.Logic.Contracts.DTO.Machine dto;
			AddNewMachine = machineID <= 0;
			if (AddNewMachine)
			{
				dto = await _machineService.DefaultMachine();
			}
			else
			{
				dto = await _machineService.Get(machineID);
			}

			Machine = dto.Convert();

			RaisePropertyChanged(nameof(Machine));

			RaisePropertyChanged(nameof(MachineCommands));
			RaisePropertyChanged(nameof(MachineInitCommands));
		}

		public async void SaveMachine()
		{
            int id;

			var m = _currentMachine.Convert();

			id = await _machineService.Update(m);

			await LoadMachine(id);
            CloseAction();
        }

		public bool CanSaveMachine()
		{
			return true;
		}

        public async void DeleteMachine()
        {
    		await _machineService.Delete(_currentMachine.Convert());
			CloseAction();
		}

		public bool CanDeleteMachine()
        {
            return !AddNewMachine;
        }

		public async void AddMachine()
        {
            await LoadMachine(-1);
        }

		public bool CanAddMachine()
		{
			return !AddNewMachine;
		}

		public async void ReadFromMachine()
		{
			if (MessageBox?.Invoke("Read configuration from machine?", "CNCLib", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
			{
				try
				{
					Com.ResetOnConnect = Global.Instance.ResetOnConnect || Machine.NeedDtr;
					Com.CommandToUpper = Machine.CommandToUpper;
					Com.BaudRate = (int)Machine.BaudRate;
					Com.Connect(Machine.ComPort);

					await Com.SendCommandAsync("?",3000);
					await Task.Delay(100);

					var eeprom = await new EepromHelper().ReadEepromAsync();
					if (eeprom != null)
					{
						Machine.Coolant = eeprom.HasCoolant;
						Machine.Rotate = eeprom.CanRotate;
						Machine.Spindle = eeprom.HasSpindle;
						Machine.SDSupport = eeprom.HasSD;
						Machine.Rotate = eeprom.CanRotate;
						Machine.Coolant = eeprom.HasCoolant;
						Machine.Laser = eeprom.IsLaser;
						Machine.Axis = (int) eeprom.UseAxis;

						Machine.SizeX = eeprom.GetAxis(0).Size / 1000m;
						Machine.SizeY = eeprom.GetAxis(1).Size / 1000m;
						Machine.SizeZ = eeprom.GetAxis(2).Size / 1000m;
						Machine.SizeA = eeprom.GetAxis(3).Size / 1000m;

                        Machine.CommandSyntax = eeprom.CommandSyntax;

						var orig = Machine;
						Machine = null;
						Machine = orig;
					}
				}
				catch (Exception e)
				{
					MessageBox?.Invoke("Open serial port failed? " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				finally
				{
					Com.Disconnect();
				}
			}
		}

		public bool CanReadFromMachine()
		{
			return true;
		}


		#endregion

		#region Commands

		public ICommand SaveMachineCommand => new DelegateCommand(SaveMachine, CanSaveMachine);
        public ICommand DeleteMachineCommand => new DelegateCommand(DeleteMachine, CanDeleteMachine);
        public ICommand AddMachineCommand => new DelegateCommand(AddMachine, CanAddMachine);

		public ICommand ReadFromMachineCommand => new DelegateCommand(ReadFromMachine, CanReadFromMachine);

		#endregion
	}
}
