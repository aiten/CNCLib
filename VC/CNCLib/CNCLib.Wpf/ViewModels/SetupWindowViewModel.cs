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
using System.Threading;
using CNCLib.Logic.Interfaces;
using CNCLib.Wpf.Models;

namespace CNCLib.Wpf.ViewModels
{
    public class SetupWindowViewModel : BaseViewModel
    {
        public SetupWindowViewModel()
		{
            LoadMachines(-1);
			ResetOnConnect = false;
		}

        private void LoadMachines(int defaultmachineid)
        {
            var machines = new ObservableCollection<Models.Machine>();

			using (var controler = LogicFactory.Create<IMachineControler>())
			{
				foreach(var m in controler.GetMachines())
				{
					machines.Add(Converter.Convert(m));
				}
				int defaultM = controler.GetDetaultMachine();

				Machines = machines;

				var defaultmachine = machines.FirstOrDefault((m) => m.MachineID == defaultmachineid);

				if (defaultmachine == null)
					defaultmachine = machines.FirstOrDefault((m) => m.MachineID == defaultM);

				if (defaultmachine == null && machines.Count > 0)
					defaultmachine = machines[0];

				Machine = defaultmachine;
			}
		}
 
        #region Properties

		private Framework.Arduino.ArduinoSerialCommunication Com
        {
			get { return Framework.Tools.Pattern.Singleton<Framework.Arduino.ArduinoSerialCommunication>.Instance; }
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

		private bool _sendInitCommands = true;
		public bool SendInitCommands
		{
			get { return _sendInitCommands; }
			set { SetProperty(ref _sendInitCommands, value); }
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

				if (SendInitCommands && Machine != null)
				{
					var initCommands = Machine.MachineInitCommands;

					if (initCommands.Count() > 0)
					{
						if (ResetOnConnect)
						{
							Com.SendCommand("");
						}

						foreach (var initcmd in initCommands.OrderBy(cmd => cmd.SeqNo))
						{
							Com.SendCommand(initcmd.CommandString);
						}
					}
				}
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

			using (var controler = LogicFactory.Create<IMachineControler>())
			{
				Global.Instance.Machine = controler.GetMachine(Machine.MachineID);
			}
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
			using (var controler = LogicFactory.Create<IMachineControler>())
			{
				controler.SetDetaultMachine(Machine.MachineID);
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

        #endregion

        #region Commands

        public ICommand SetupMachineCommand { get { return new DelegateCommand(SetupMachine, CanSetupMachine); } }
 		public ICommand ConnectCommand { get { return new DelegateCommand(Connect, CanConnect); } }
		public ICommand DisConnectCommand	{ get { return new DelegateCommand(DisConnect, CanDisConnect); } }
		public ICommand SetDefaultMachineCommand { get { return new DelegateCommand(SetDefaultMachine, CanSetupMachine); } }

        #endregion
    }
}
