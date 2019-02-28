/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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
using System.Threading.Tasks;
using System.Windows.Input;

using CNCLib.WpfClient.Services;

using Framework.Pattern;
using Framework.Wpf.Helpers;
using Framework.Wpf.ViewModels;

namespace CNCLib.WpfClient.ViewModels
{
    public class JoystickViewModel : BaseViewModel, IDisposable
    {
        private readonly IFactory<IJoystickService> _joystickService;

        #region crt

        public JoystickViewModel(IFactory<IJoystickService> joystickService)
        {
            _joystickService = joystickService ?? throw new ArgumentNullException();
        }

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

        public Models.Joystick Joystick
        {
            get => _currentJoystick;
            set { SetProperty(() => _currentJoystick == value, () => _currentJoystick = value); }
        }

        #endregion

        #region Operations

        public async Task LoadJoystick()
        {
            using (var scope = _joystickService.Create())
            {
                var joystick = await scope.Instance.Load();
                _id      = joystick.Item2;
                Joystick = joystick.Item1;

                RaisePropertyChanged(nameof(Joystick));
            }
        }

        public async void SaveJoystick()
        {
            using (var scope = _joystickService.Create())
            {
                _id = await scope.Instance.Save(_currentJoystick, _id);
                CloseAction();
            }
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