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
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Wpf.Models;
using Framework.Tools.Dependency;

namespace CNCLib.Wpf.ViewModels
{
	public class JoystickViewModel : BaseViewModel, IDisposable
	{
        public JoystickViewModel()
		{
			LoadJoystick();
		}
		public void Dispose()
		{
		}

		public void LoadJoystick()
        {
			ComPort = "com7";
			BaudRate = 250000;
			_id = -1;

			using (var controller = Dependency.Resolve<IItemController>())
			{
				var joystick = controller.GetAll(typeof(Models.Joystick));
				if (joystick != null && joystick.Count() > 0) 
				{
					_id = joystick.First().ItemID;
					_currentJoystick = (Models.Joystick)controller.Create(_id);
				}
			}

			OnPropertyChanged(() => ComPort);
			OnPropertyChanged(() => BaudRate);
		}

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

		public void SaveJoystick()
		{
			using (var controller = Dependency.Resolve<IItemController>())
			{
				if (_id >= 0)
				{
					controller.Save(_id,"Joystick", _currentJoystick);
				}
				else
				{
					_id = controller.Add("Joystick", _currentJoystick);
				}
			}
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
