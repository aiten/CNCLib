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
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;
using CNCLib.Wpf.Helpers;

namespace CNCLib.Wpf.ViewModels
{
	public class JoystickViewModel : BaseViewModel, IDisposable
	{

		#region crt

		public JoystickViewModel()
		{
			LoadJoystick();
		}

		#endregion

		#region dispose

		public void Dispose()
		{
		}

		#endregion

		#region Properties

		Models.Joystick _currentJoystick = new Models.Joystick();
		int _id = -1;

        public string ComPort
        {
			get { return _currentJoystick.ComPort; }
            set { SetProperty(() => _currentJoystick.ComPort == value, () => _currentJoystick.ComPort = value); }
        }

		public int BaudRate
		{
			get { return _currentJoystick.BaudRate; }
            set { SetProperty(() => _currentJoystick.BaudRate == value, () => _currentJoystick.BaudRate = value); }
		}

		#endregion

		#region Operations
		public void LoadJoystick()
		{
			_currentJoystick = JoystickHelper.Load(out _id);

			OnPropertyChanged(() => ComPort);
			OnPropertyChanged(() => BaudRate);
		}

		public void SaveJoystick()
		{
			JoystickHelper.Save(_currentJoystick, ref _id);
			CloseAction();
        }

		public bool CanSaveJoystick()
		{
			return true;
		}

		#endregion

		#region Commands

		public ICommand SaveJoystickCommand { get { return new DelegateCommand(SaveJoystick, CanSaveJoystick); } }

        #endregion
    }
}
