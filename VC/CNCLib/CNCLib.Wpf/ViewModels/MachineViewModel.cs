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
using Framework.Tools;
using CNCLib.Logic.Contracts;
using CNCLib.Wpf.Models;
using Framework.Tools.Dependency;
using CNCLib.ServiceProxy;
using System.Threading.Tasks;

namespace CNCLib.Wpf.ViewModels
{
	public class MachineViewModel : BaseViewModel, IDisposable
	{
		#region crt

		public MachineViewModel()
		{
            AddNewMachine = false;
		}

		#endregion

		#region dispose

		public void Dispose()
		{
		}

		#endregion

		#region Properties

		Models.Machine _currentMachine = new Models.Machine();

        public string MachineName
        {
            get { return _currentMachine.Name; }
            set { SetProperty(() => _currentMachine.Name == value, () => _currentMachine.Name = value); }
        }

        public string ComPort
        {
			get { return _currentMachine.ComPort; }
            set { SetProperty(() => _currentMachine.ComPort == value, () => _currentMachine.ComPort = value); }
        }

		public int BaudRate
		{
			get { return _currentMachine.BaudRate; }
            set { SetProperty(() => _currentMachine.BaudRate == value, () => _currentMachine.BaudRate = value); }
		}
		public int Axis
		{
			get { return _currentMachine.Axis; }
			set { SetProperty(() => _currentMachine.Axis == value, () => _currentMachine.Axis = value); }
		}

		public bool CommandToUpper
		{
			get { return _currentMachine.CommandToUpper; }
			set { SetProperty(() => _currentMachine.CommandToUpper == value, () => _currentMachine.CommandToUpper = value); }
		}

		public int BufferSize
		{
			get { return _currentMachine.BufferSize; }
			set { SetProperty(() => _currentMachine.BufferSize == value, () => _currentMachine.BufferSize = value); }
		}

		public decimal SizeX
		{
			get { return _currentMachine.SizeX; }
			set { SetProperty(() => _currentMachine.SizeX == value, () => _currentMachine.SizeX = value); }
		}
		public decimal SizeY
		{
			get { return _currentMachine.SizeY; }
			set { SetProperty(() => _currentMachine.SizeY == value, () => _currentMachine.SizeY = value); }
		}
		public decimal SizeZ
		{
			get { return _currentMachine.SizeZ; }
            set { SetProperty(() => _currentMachine.SizeZ == value, () => _currentMachine.SizeZ = value); }
		}
		public decimal SizeA
		{
			get { return _currentMachine.SizeA; }
			set { SetProperty(() => _currentMachine.SizeA == value, () => _currentMachine.SizeA = value); }
		}
		public decimal SizeB
		{
			get { return _currentMachine.SizeB; }
			set { SetProperty(() => _currentMachine.SizeB == value, () => _currentMachine.SizeB = value); }
		}
		public decimal SizeC
		{
			get { return _currentMachine.SizeC; }
			set { SetProperty(() => _currentMachine.SizeC == value, () => _currentMachine.SizeC = value); }
		}
		public decimal ProbeSizeX
		{
			get { return _currentMachine.ProbeSizeX; }
			set { SetProperty(() => _currentMachine.ProbeSizeX == value, () => _currentMachine.ProbeSizeX = value); }
		}
		public decimal ProbeSizeY
		{
			get { return _currentMachine.ProbeSizeY; }
			set { SetProperty(() => _currentMachine.ProbeSizeY == value, () => _currentMachine.ProbeSizeY = value); }
		}
		public decimal ProbeSizeZ
		{
			get { return _currentMachine.ProbeSizeZ; }
			set { SetProperty(() => _currentMachine.ProbeSizeZ == value, () => _currentMachine.ProbeSizeZ = value); }
		}
		public decimal ProbeDistUp
		{
			get { return _currentMachine.ProbeDistUp; }
			set { SetProperty(() => _currentMachine.ProbeDistUp == value, () => _currentMachine.ProbeDistUp = value); }
		}
		public decimal ProbeDist
		{
			get { return _currentMachine.ProbeDist; }
			set { SetProperty(() => _currentMachine.ProbeDist == value, () => _currentMachine.ProbeDist = value); }
		}
		public decimal ProbeFeed
		{
			get { return _currentMachine.ProbeFeed; }
			set { SetProperty(() => _currentMachine.ProbeFeed == value, () => _currentMachine.ProbeFeed = value); }
		}
		public bool SDSupport
		{
			get { return _currentMachine.SDSupport; }
			set { SetProperty(() => _currentMachine.SDSupport == value, () => _currentMachine.SDSupport = value); }
		}
		public bool Spindle
		{
			get { return _currentMachine.Spindle; }
			set { SetProperty(() => _currentMachine.Spindle == value, () => _currentMachine.Spindle = value); }
		}
		public bool Coolant
		{
			get { return _currentMachine.Coolant; }
			set { SetProperty(() => _currentMachine.Coolant == value, () => _currentMachine.Coolant = value); }
		}
        public bool Laser
        {
            get { return _currentMachine.Laser; }
            set { SetProperty(() => _currentMachine.Laser == value, () => _currentMachine.Laser = value); }
        }
        public bool Rotate
		{
			get { return _currentMachine.Rotate; }
			set { SetProperty(() => _currentMachine.Rotate == value, () => _currentMachine.Rotate = value); }
		}

		public ObservableCollection<Models.MachineCommand> MachineCommands
		{
			get { return _currentMachine.MachineCommands; }
				}


		public ObservableCollection<Models.MachineInitCommand> MachineInitCommands
		{
			get { return _currentMachine.MachineInitCommands; }
		}

		public bool AddNewMachine { get; set; }

		#endregion

		#region Operations

		public async Task LoadMachine(int machineID)
		{
			CNCLib.Logic.Contracts.DTO.Machine dto;
			using (var controller = Dependency.Resolve<IMachineService>())
			{
				AddNewMachine = machineID <= 0;
				if (AddNewMachine)
				{
					dto = await controller.DefaultMachine();
				}
				else
				{
					dto = await controller.Get(machineID);
				}
			}

			_currentMachine = dto.Convert();

			OnPropertyChanged(() => MachineName);
			OnPropertyChanged(() => ComPort);
			OnPropertyChanged(() => Axis);
			OnPropertyChanged(() => BaudRate);
			OnPropertyChanged(() => CommandToUpper);
			OnPropertyChanged(() => SizeX);
			OnPropertyChanged(() => SizeY);
			OnPropertyChanged(() => SizeZ);
			OnPropertyChanged(() => SizeA);
			OnPropertyChanged(() => SizeB);
			OnPropertyChanged(() => SizeC);
			OnPropertyChanged(() => BufferSize);
			OnPropertyChanged(() => ProbeSizeX);
			OnPropertyChanged(() => ProbeSizeY);
			OnPropertyChanged(() => ProbeSizeZ);
			OnPropertyChanged(() => ProbeDist);
			OnPropertyChanged(() => ProbeDistUp);
			OnPropertyChanged(() => ProbeFeed);
			OnPropertyChanged(() => SDSupport);
			OnPropertyChanged(() => Spindle);
			OnPropertyChanged(() => Coolant);
			OnPropertyChanged(() => Laser);
			OnPropertyChanged(() => Rotate);

			OnPropertyChanged(() => MachineCommands);
			OnPropertyChanged(() => MachineInitCommands);
		}

		public async void SaveMachine()
		{
            int id;

			var m = _currentMachine.Convert();

			using (var controller = Dependency.Resolve<IMachineService>())
			{
				id = await controller.Update(m);
			}

			await LoadMachine(id);
            CloseAction();
        }

		public bool CanSaveMachine()
		{
			return true;
		}

        public async void DeleteMachine()
        {
			using (var controller = Dependency.Resolve<IMachineService>())
			{
				await controller.Delete(_currentMachine.Convert());
			}
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

		#endregion

		#region Commands

		public ICommand SaveMachineCommand { get { return new DelegateCommand(SaveMachine, CanSaveMachine); } }
        public ICommand DeleteMachineCommand { get { return new DelegateCommand(DeleteMachine, CanDeleteMachine); } }
        public ICommand AddMachineCommand { get { return new DelegateCommand(AddMachine, CanAddMachine); } }

        #endregion
    }
}
