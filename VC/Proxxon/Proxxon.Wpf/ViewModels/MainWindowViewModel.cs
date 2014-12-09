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
using Proxxon.Wpf.Models;


namespace Proxxon.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
		public MainWindowViewModel()
		{
			var machines = new ObservableCollection<Machine>(new Machine[] { 
				new Models.Machine()
				{
					Name = "Proxxon",
					ComPort = "com4",
					BaudRate = 115200,
					SizeX = 130m,
					SizeY = 45m,
					SizeZ = 81m,
					BufferSize = 63,
					CommandToUpper = false
				},
				new Models.Machine()
				{
					Name = "KK1000S",
					ComPort = "com11",
					BaudRate = 115200,
					SizeX = 830m,
					SizeY = 500m,
					SizeZ = 100m,
					BufferSize = 63,
					CommandToUpper = false
				}});

			Machines = machines;
			Machine = Machines[0];

			ResetOnConnect = false;
		}
        #region Properties

		private Framework.Logic.ArduinoSerialCommunication Com
        {
			get { return Framework.Tools.Singleton<Framework.Logic.ArduinoSerialCommunication>.Instance; }
        }

		#region Current Machine

		public Machine Machine
		{
			get { return _currentMachine; }
			set { 
					AssignProperty(ref _currentMachine, value);

					OnPropertyChanged(() => ComPort);
					OnPropertyChanged(() => BaudRate);
					OnPropertyChanged(() => CommandToUpper);
					OnPropertyChanged(() => SizeX);
					OnPropertyChanged(() => SizeY);
					OnPropertyChanged(() => SizeZ);
					OnPropertyChanged(() => BufferSize);
 
			}
		}

		Models.Machine _currentMachine;
		private ObservableCollection<Machine> _machines;
		public ObservableCollection<Machine> Machines
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
			set { SetProperty(_currentMachine.ComPort, value); }
        }

		public uint BaudRate
		{
			get { return _currentMachine.BaudRate; }
			set { SetProperty(_currentMachine.BaudRate, value); }
		}

		public bool CommandToUpper
		{
			get { return _currentMachine.CommandToUpper; }
			set { SetProperty(_currentMachine.CommandToUpper, value); }
		}

		public int BufferSize
		{
			get { return _currentMachine.BufferSize; }
			set { int buffersize = Com.ArduinoBuffersize; SetProperty( buffersize, value); Com.ArduinoBuffersize = _currentMachine.BufferSize = buffersize; }
		}

		public decimal SizeX
		{
			get { return _currentMachine.SizeX; }
			set { SetProperty(_currentMachine.SizeX, value); }
		}
		public decimal SizeY
		{
			get { return _currentMachine.SizeY; }
			set { SetProperty(_currentMachine.SizeY, value); }
		}
		public decimal SizeZ
		{
			get { return _currentMachine.SizeZ; }
			set { SetProperty(_currentMachine.SizeZ, value); }
		}

		#endregion

		private bool _resetOnConnect;
		public bool ResetOnConnect
		{
			get { return _resetOnConnect; }
			set { SetProperty(_resetOnConnect, value); }
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
