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
		}
 
        #region Properties

		private Framework.Logic.ArduinoSerialCommunication Com
        {
			get { return Framework.Tools.Singleton<Framework.Logic.ArduinoSerialCommunication>.Instance; }
        }

		#region Current Machine

        public bool Connected
        {
            get { return Com.IsConnected; }
        }

		#endregion

        #endregion

        #region Operations

        #endregion

        #region Commands
/*
        public ICommand SetupMachineCommand { get { return new DelegateCommand(SetupMachine, CanSetupMachine); } }
 		public ICommand ConnectCommand { get { return new DelegateCommand(Connect, CanConnect); } }
		public ICommand DisConnectCommand	{ get { return new DelegateCommand(DisConnect, CanDisConnect); } }
		public ICommand ManualControlCommand	{ get { return new DelegateCommand(ShowManualControl, CanShowManualControl); } }
        public ICommand PaintCommand { get { return new DelegateCommand(ShowPaint, CanShowPaint); } }
		public ICommand SetDefaultMachineCommand { get { return new DelegateCommand(SetDefaultMachine, CanSetupMachine); } }
*/
        #endregion
    }
}
