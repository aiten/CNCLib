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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;
using System.Windows;
using Proxxon.Wpf;
using Framework.Tools;
using System.Linq.Expressions;
using Proxxon.Logic;
using Proxxon.GCode;


namespace Proxxon.Wpf.ViewModels
{
    public class MachineViewModel : BaseViewModel
    {
        public MachineViewModel()
		{
            AddNewMachine = false;
		}

        public void LoadMachine(int machineID)
        {
            AddNewMachine = machineID <= 0;
			MachineCommands.Clear();
            if (AddNewMachine)
            {
                _currentMachine = new Models.Machine()
                {
                    Name = "New",
                    ComPort = "comX",
                    SizeX = 130m,
                    SizeY = 45m,
                    SizeZ = 81m,
                    BaudRate = 115200,
                    BufferSize = 63,
                    CommandToUpper = false,
                    Default = false,
					ProbeSizeZ = 25
                };
            }
            else
            {
                _currentMachine = ObjectConverter.NewCloneProperties<Models.Machine, Proxxon.Logic.DTO.Machine>(new MachineControler().GetMachine(machineID));
				MachineCommands.AddCloneProperties(new MachineControler().GetMachineCommands(machineID));
//MachineCommands.Add(new Models.MachineCommand() { MachineID = machineID, CommandString = "Neu" });
//MachineCommands.RemoveAt(MachineCommands.Count-1);
			}

            OnPropertyChanged(() => MachineName);
            OnPropertyChanged(() => ComPort);
            OnPropertyChanged(() => BaudRate);
            OnPropertyChanged(() => CommandToUpper);
            OnPropertyChanged(() => SizeX);
            OnPropertyChanged(() => SizeY);
            OnPropertyChanged(() => SizeZ);
            OnPropertyChanged(() => BufferSize);
            OnPropertyChanged(() => Default);
			OnPropertyChanged(() => ProbeSizeX);
			OnPropertyChanged(() => ProbeSizeY);
			OnPropertyChanged(() => ProbeSizeZ);
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
        public bool Default
        {
            get { return _currentMachine.Default; }
            set { SetProperty(() => _currentMachine.Default == value, () => _currentMachine.Default = value); }
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

		private ObservableCollection<Models.MachineCommand> _MachineCommands;

		public ObservableCollection<Models.MachineCommand> MachineCommands
		{
			get
			{
				if (_MachineCommands==null)
				{
					_MachineCommands = new ObservableCollection<Models.MachineCommand>();
					_MachineCommands.CollectionChanged += ((sender, e) =>
							{
								if (e.NewItems != null)
								{
									foreach (Models.MachineCommand item in e.NewItems)
									{
										item.MachineID = _currentMachine.MachineID;
									}
								}
							});
				}
				return _MachineCommands;
			}
		}

		#endregion

        public bool AddNewMachine { get; set; }
        
        #region Operations

		public void SaveMachine()
		{
            int id;

var m = _currentMachine.NewCloneProperties<Proxxon.Logic.DTO.Machine, Models.Machine>();
m.MachineCommands = MachineCommands.ToArray().CloneProperties<Proxxon.Logic.DTO.MachineCommand, Models.MachineCommand>().ToList();

id = new MachineControler().StoreMachine(m);
/*
            if (AddNewMachine)
            {
                id = new MachineControler().Add(_currentMachine.NewCloneProperties<Proxxon.Logic.DTO.Machine, Models.Machine>());
            }
            else
            {
                id = _currentMachine.MachineID;
                new MachineControler().Update(_currentMachine.NewCloneProperties<Proxxon.Logic.DTO.Machine, Models.Machine>());
            }
*/
//			new MachineControler().StoreMachine(
//				_currentMachine.NewCloneProperties<Proxxon.Logic.DTO.Machine, Models.Machine>(),
//				MachineCommands.ToArray().CloneProperties<Proxxon.Logic.DTO.MachineCommand, Models.MachineCommand>()
//				);

			LoadMachine(id);
            ViewWindow.Close();
        }
		public bool CanSaveMachine()
		{
			return true;
		}

        public void DeleteMachine()
        {
            new MachineControler().Delete(_currentMachine.NewCloneProperties<Proxxon.Logic.DTO.Machine, Models.Machine>());
            ViewWindow.Close();
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
