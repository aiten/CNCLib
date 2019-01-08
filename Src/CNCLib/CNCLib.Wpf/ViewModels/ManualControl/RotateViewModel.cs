////////////////////////////////////////////////////////
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

using CNCLib.Wpf.Helpers;

using Framework.Wpf.Helpers;
using Framework.Arduino.SerialCommunication;

namespace CNCLib.Wpf.ViewModels.ManualControl
{
    public class RotateViewModel : DetailViewModel
    {
        private readonly Global _global;

        public RotateViewModel(IManualControlViewModel vm, Global global) : base(vm, global)
        {
            _global = global ?? throw new ArgumentNullException();
        }

        #region Properties

        #endregion

        #region Commands / CanCommands

        public bool CanSendRotate()
        {
            return CanSend() && _global.Machine.Rotate;
        }

        public void SendG69()
        {
            RunAndUpdate(() => { _global.Com.Current.PrepareAndQueueCommand(_global.Machine, "g69"); });
        }

        public void SendG68X0Y0R90()
        {
            RunAndUpdate(() => { _global.Com.Current.PrepareAndQueueCommand(_global.Machine, "g68 x0y0r90"); });
        }

        public void SendG68X0Y0R270()
        {
            RunAndUpdate(() => { _global.Com.Current.PrepareAndQueueCommand(_global.Machine, "g68 x0y0r270"); });
        }

        public void SendG6810()
        {
            RunAndUpdate(() => { _global.Com.Current.PrepareAndQueueCommand(_global.Machine, "g68.10"); });
        }

        public void SendG6811()
        {
            RunAndUpdate(() => { _global.Com.Current.PrepareAndQueueCommand(_global.Machine, "g68.11"); });
        }

        public void SendG6813()
        {
            RunAndUpdate(() => { _global.Com.Current.PrepareAndQueueCommand(_global.Machine, "g68.13 j0k0"); });
        }

        public void SendG6814()
        {
            RunAndUpdate(() => { _global.Com.Current.PrepareAndQueueCommand(_global.Machine, "g68.14 i0"); });
        }

        #endregion

        #region ICommand

        public ICommand SendG69Command         => new DelegateCommand(SendG69,         CanSendRotate);
        public ICommand SendG68X0Y0R90Command  => new DelegateCommand(SendG68X0Y0R90,  CanSendRotate);
        public ICommand SendG68X0Y0R270Command => new DelegateCommand(SendG68X0Y0R270, CanSendRotate);

        public ICommand SendG6810Command => new DelegateCommand(SendG6810, CanSendRotate);
        public ICommand SendG6811Command => new DelegateCommand(SendG6811, CanSendRotate);
        public ICommand SendG6813Command => new DelegateCommand(SendG6813, CanSendRotate);
        public ICommand SendG6814Command => new DelegateCommand(SendG6814, CanSendRotate);

        #endregion
    }
}