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


namespace Proxxon.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
		public MainWindowViewModel()
		{
			BaudRate = 115200;
			ComPort = "com4";
			CommandToUpper = false;
			ResetOnConnect = false;
		}
        #region Properties

		private Framework.Logic.ArduinoSerialCommunication Com
        {
			get { return Framework.Tools.Singleton<Framework.Logic.ArduinoSerialCommunication>.Instance; }
        }

        public bool Connected
        {
            //get { return true; }
            get { return Com.IsConnected; }
        }

        private string _comport;
        public string ComPort
        {
            get { return string.IsNullOrEmpty(_comport) ? "com4" : _comport; }
            set { SetProperty(ref _comport,value); }
        }

		private bool _resetOnConnect;
		public bool ResetOnConnect
		{
			get { return _resetOnConnect; }
            set { SetProperty(ref _resetOnConnect,value); }
		}

		private uint _baudRate;
		public uint BaudRate
		{
			get { return _baudRate; }
			set { SetProperty(ref _baudRate, value); }
		}

		private bool _commandToUpper;
		public bool CommandToUpper
		{
			get { return _commandToUpper; }
			set { SetProperty(ref _commandToUpper, value); }
		}

		public int BufferSize
		{
			get { return Com.ArduinoBuffersize; }
			set { int buffersize = Com.ArduinoBuffersize; SetProperty(ref buffersize, value); Com.ArduinoBuffersize = buffersize; }
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

        #endregion

        #region Commands

		public ICommand ConnectCommand { get { return new DelegateCommand(Connect, CanConnect); } }
		public ICommand DisConnectCommand	{ get { return new DelegateCommand(DisConnect, CanDisConnect); } }

        #endregion
    }
}
