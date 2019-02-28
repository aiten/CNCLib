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
using System.Windows.Input;

using Framework.Arduino.SerialCommunication;
using Framework.Wpf.Helpers;

namespace CNCLib.WpfClient.ViewModels.ManualControl
{
    public class ShiftViewModel : DetailViewModel
    {
        private readonly Global _global;

        public ShiftViewModel(IManualControlViewModel vm, Global global) : base(vm, global)
        {
            _global = global ?? throw new ArgumentNullException();
        }

        #region Properties

        #endregion

        #region Commands / CanCommands

        public void SendG92()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand("g92"); });
        }

        #endregion

        #region ICommand

        public ICommand SendG92Command => new DelegateCommand(SendG92, CanSendGCode);

        #endregion
    }
}