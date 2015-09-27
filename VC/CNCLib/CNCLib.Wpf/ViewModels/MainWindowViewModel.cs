////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
using CNCLib.Wpf;
using Framework.Tools;
using System.Linq.Expressions;
using CNCLib.Logic;
using CNCLib.GCode;


namespace CNCLib.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel()
		{
            LoadMachines(-1);
			ResetOnConnect = false;
		}

        private void LoadMachines(int defaultmachineid)
        {
            var machines = new ObservableCollection<Models.Machine>();

            machines.AddCloneProperties(new MachineControler().GetMachines());
			int defaultM = new MachineControler().GetDetaultMachine();			
			
			Machines = machines;

            var defaultmachine = machines.FirstOrDefault((m) => m.MachineID == defaultmachineid);

            if (defaultmachine == null)
				defaultmachine = machines.FirstOrDefault((m) => m.MachineID == defaultM);

            if (defaultmachine == null && machines.Count > 0)
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
			ObjectConverter.CopyProperties(Settings.Instance, Machine);
            Com.ArduinoBuffersize = Machine.BufferSize;
			Global.Instance.Machine = new MachineControler().GetMachine(Machine.MachineID);
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

       public void SetupMachine()
        {
            var dlg = new MachineView();

            var vm = dlg.DataContext as MachineViewModel;
            var mID = Machine!=null ? Machine.MachineID : -1; 
            vm.LoadMachine(mID);
            dlg.ShowDialog();

            LoadMachines(mID);
        }

	   public void SetDefaultMachine()
	   {
		   new MachineControler().SetDetaultMachine(Machine.MachineID);
	   }

		public bool CanSetupMachine()
        {
            return true;
        }

        public void ShowManualControl()
        {
			new CNCLib.Wpf.ManualControl().ShowDialog();
        }

        public bool CanShowManualControl()
        {
            return true;
        }

        public void ShowPaint()
        {
            using (CNCLib.GUI.PaintForm form = new CNCLib.GUI.PaintForm())
            {
                form.ShowDialog();
            }
        }

        public bool CanShowPaint()
        {
            return true;
        }

        #endregion

        #region Commands

        public ICommand SetupMachineCommand { get { return new DelegateCommand(SetupMachine, CanSetupMachine); } }
 		public ICommand ConnectCommand { get { return new DelegateCommand(Connect, CanConnect); } }
		public ICommand DisConnectCommand	{ get { return new DelegateCommand(DisConnect, CanDisConnect); } }
		public ICommand ManualControlCommand	{ get { return new DelegateCommand(ShowManualControl, CanShowManualControl); } }
        public ICommand PaintCommand { get { return new DelegateCommand(ShowPaint, CanShowPaint); } }
		public ICommand SetDefaultMachineCommand { get { return new DelegateCommand(SetDefaultMachine, CanSetupMachine); } }

        #endregion
    }
}
