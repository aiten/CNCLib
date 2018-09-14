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
using System.Threading.Tasks;

namespace CNCLib.Wpf.ViewModels
{
    public class JoystickViewModel : BaseViewModel, IDisposable
    {
        #region crt

        public override async Task Loaded()
        {
            await base.Loaded();
            await LoadJoystick();
        }

        #endregion

        #region dispose

        public void Dispose()
        {
        }

        #endregion

        #region Properties

        Models.Joystick _currentJoystick = new Models.Joystick();
        int             _id              = -1;

        public Models.Joystick Joystick { get => _currentJoystick; set { SetProperty(() => _currentJoystick == value, () => _currentJoystick = value); } }

        #endregion

        #region Operations

        public async Task LoadJoystick()
        {
            var joystick = await JoystickHelper.Load();
            _id      = joystick.Item2;
            Joystick = joystick.Item1;

            RaisePropertyChanged(nameof(Joystick));
        }

        public async void SaveJoystick()
        {
            _id = await JoystickHelper.Save(_currentJoystick, _id);
            CloseAction();
        }

        public bool CanSaveJoystick()
        {
            return true;
        }

        #endregion

        #region Commands

        public ICommand SaveJoystickCommand => new DelegateCommand(SaveJoystick, CanSaveJoystick);

        #endregion
    }
}