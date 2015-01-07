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
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel()
		{
            LoadMachines();

			ResetOnConnect = false;
		}

        private void LoadMachines()
        {
            var machines = new ObservableCollection<Models.Machine>();

            machines.AddCloneProperties(new MachineControler().GetMachines());

            Machines = machines;
            Machine = Machines[0];
        }
 
        #region Properties

		private Framework.Logic.ArduinoSerialCommunication Com
        {
			get { return Framework.Tools.Singleton<Framework.Logic.ArduinoSerialCommunication>.Instance; }
        }

		#region Current Machine

        public Models.Machine Machine
		{
            get { return _selectedMachine; }
			set {
                    _selectedMachine = value;
/*
                    AssignProperty(ref _currentMachine, value.CloneProperties());

					OnPropertyChanged(() => ComPort);
					OnPropertyChanged(() => BaudRate);
					OnPropertyChanged(() => CommandToUpper);
					OnPropertyChanged(() => SizeX);
					OnPropertyChanged(() => SizeY);
					OnPropertyChanged(() => SizeZ);
					OnPropertyChanged(() => BufferSize);
*/			
					AssignProperty(() => 
                    {
                        if (value == null)
                            _currentMachine = new Models.Machine();
                        else
							_currentMachine = value.CloneProperties();
						OnPropertyChanged(() => ComPort);
						OnPropertyChanged(() => BaudRate);
						OnPropertyChanged(() => CommandToUpper);
						OnPropertyChanged(() => SizeX);
						OnPropertyChanged(() => SizeY);
						OnPropertyChanged(() => SizeZ);
						OnPropertyChanged(() => BufferSize);
					}
				);
			}
		}

		Models.Machine _currentMachine;
        Models.Machine _selectedMachine;

        private ObservableCollection<Models.Machine> _machines;
        public ObservableCollection<Models.Machine> Machines
		{
			get { return _machines; }
			set { AssignProperty(ref _machines, value); }
		}

        public bool Connected
        {
            //get { return true; }
            get { return Com.IsConnected; }
        }

        public string ComPort
        {
			get { return string.IsNullOrEmpty(_currentMachine.ComPort) ? "com4" : _currentMachine.ComPort; }
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
			set { int buffersize = Com.ArduinoBuffersize; SetProperty(ref  buffersize, value); Com.ArduinoBuffersize = _currentMachine.BufferSize = buffersize; }
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

		#endregion

		private bool _resetOnConnect=true;
		public bool ResetOnConnect
		{
			get { return _resetOnConnect; }
			set { SetProperty(ref _resetOnConnect, value); }
		}

        #endregion

        #region Operations

        public void Connect()
        {
			try
			{
				Com.ResetOnConnect = ResetOnConnect;
				Com.CommandToUpper = CommandToUpper;
				Com.BaudRate = (int) BaudRate;
				Com.Connect(ComPort);
                Settings.Instance.SizeX = SizeX;
                Settings.Instance.SizeY = SizeY;
                Settings.Instance.SizeZ = SizeZ;
			}
			catch(Exception e)
			{
				MessageBox.Show("Open serial port failed? " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			OnPropertyChanged(() => Connected);
		}
		public bool CanConnect()
        {
            return !Connected;
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

		public void SaveMachine()
		{
            _selectedMachine.CopyProperties(_currentMachine);
            new MachineControler().Update(_selectedMachine.NewCloneProperties<Proxxon.Logic.DTO.Machine, Models.Machine>());
            LoadMachines();
		}
		public bool CanSaveMachine()
		{
			return true;
		}

        #endregion

        #region Commands

		public ICommand SaveMachineCommand { get { return new DelegateCommand(SaveMachine, CanSaveMachine); } }
		public ICommand ConnectCommand { get { return new DelegateCommand(Connect, CanConnect); } }
		public ICommand DisConnectCommand	{ get { return new DelegateCommand(DisConnect, CanDisConnect); } }

        #endregion
    }
}
