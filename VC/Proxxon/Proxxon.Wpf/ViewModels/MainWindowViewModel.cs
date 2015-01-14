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

            var defaultmachine = machines.First((m) => m.Default);
            if (defaultmachine == null)
                defaultmachine = machines[0];
            Machine = defaultmachine;
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
                    AssignProperty(ref _selectedMachine, value);
                    if (value!=null)
                        SetGlobal();
                }
		}

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
                Com.CommandToUpper = Machine.CommandToUpper;
                Com.BaudRate = (int)Machine.BaudRate;
                Com.Connect(Machine.ComPort);
                SetGlobal();
			}
			catch(Exception e)
			{
				MessageBox.Show("Open serial port failed? " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			OnPropertyChanged(() => Connected);
		}

        private void SetGlobal()
        {
            Settings.Instance.SizeX = Machine.SizeX;
            Settings.Instance.SizeY = Machine.SizeY;
            Settings.Instance.SizeZ = Machine.SizeZ;
            Com.ArduinoBuffersize = Machine.BufferSize;
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

       public void SetupMachine()
        {
            var dlg = new MachineView();

            var vm = dlg.DataContext as MachineViewModel;
            vm.LoadMachine(Machine);
            dlg.ShowDialog();

            LoadMachines();
        }
        public bool CanSetupMachine()
        {
            return true;
        }

        #endregion

        #region Commands

        public ICommand SetupMachineCommand { get { return new DelegateCommand(SetupMachine, CanSetupMachine); } }
 		public ICommand ConnectCommand { get { return new DelegateCommand(Connect, CanConnect); } }
		public ICommand DisConnectCommand	{ get { return new DelegateCommand(DisConnect, CanDisConnect); } }

        #endregion
    }
}
