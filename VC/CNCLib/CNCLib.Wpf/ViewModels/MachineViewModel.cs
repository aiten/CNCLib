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
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;
using Framework.Tools;
using CNCLib.Logic;
using CNCLib.Logic.Interfaces;


namespace CNCLib.Wpf.ViewModels
{
	public class MachineViewModel : BaseViewModel, IDisposable
	{
        public MachineViewModel()
		{
            AddNewMachine = false;
		}
		public void Dispose()
		{
		}

		public void LoadMachine(int machineID)
        {
            AddNewMachine = machineID <= 0;
			MachineCommands.Clear();
			MachineInitCommands.Clear();
			if (AddNewMachine)
            {
                _currentMachine = new Models.Machine()
                {
                    Name = "New",
                    ComPort = "comX",
					Axis = 3,
					SizeX = 130m,
                    SizeY = 45m,
                    SizeZ = 81m,
					SizeA = 360m,
					SizeB = 360m,
					SizeC = 360m,
					BaudRate = 115200,
                    BufferSize = 63,
                    CommandToUpper = false,
					ProbeSizeZ = 25,
					ProbeDist = 10m,
					ProbeDistUp = 3m,
					ProbeFeed = 100m,
					SDSupport = true,
					Spindle = true,
					Coolant = true,				
					Rotate = true	
                };
            }
            else
            {
				using (var controler = LogicFactory.Create<IMachineControler>())
				{
					var dto = controler.GetMachine(machineID);
                    _currentMachine = ObjectConverter.NewCloneProperties<Models.Machine, CNCLib.Logic.DTO.Machine>(dto);
					_currentMachine.MachineCommands.CloneProperties(dto.MachineCommands);
					_currentMachine.MachineInitCommands.CloneProperties(dto.MachineInitCommands);
				}
			}

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
			OnPropertyChanged(() => Rotate);
		}

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

		#endregion

		public bool AddNewMachine { get; set; }
        
        #region Operations

		public void SaveMachine()
		{
            int id;

			var m = _currentMachine.NewCloneProperties<CNCLib.Logic.DTO.Machine, Models.Machine>();
			m.MachineCommands = MachineCommands.ToArray().CloneProperties<CNCLib.Logic.DTO.MachineCommand, Models.MachineCommand>().ToList();
			m.MachineInitCommands = MachineInitCommands.ToArray().CloneProperties<CNCLib.Logic.DTO.MachineInitCommand, Models.MachineInitCommand>().ToList();

			using (var controler = LogicFactory.Create<IMachineControler>())
			{
				id = controler.StoreMachine(m);
			}

			LoadMachine(id);
            CloseAction();
        }
		public bool CanSaveMachine()
		{
			return true;
		}

        public void DeleteMachine()
        {
			using (var controler = LogicFactory.Create<IMachineControler>())
			{
				controler.Delete(_currentMachine.NewCloneProperties<CNCLib.Logic.DTO.Machine, Models.Machine>());
			}
			CloseAction();
		}
		public bool CanDeleteMachine()
        {
            return !AddNewMachine;
        }
        public void AddMachine()
        {
            LoadMachine(-1);
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
