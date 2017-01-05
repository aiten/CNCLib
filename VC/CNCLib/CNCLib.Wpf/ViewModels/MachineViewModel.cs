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

		public Models.Machine Machine
		{
			get { return _currentMachine; }
			set { SetProperty(() => _currentMachine == value, () => _currentMachine = value); }
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

			Machine = dto.Convert();

			OnPropertyChanged(() => Machine);

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

		public ICommand SaveMachineCommand => new DelegateCommand(SaveMachine, CanSaveMachine);
        public ICommand DeleteMachineCommand => new DelegateCommand(DeleteMachine, CanDeleteMachine);
        public ICommand AddMachineCommand => new DelegateCommand(AddMachine, CanAddMachine);

        #endregion
    }
}
