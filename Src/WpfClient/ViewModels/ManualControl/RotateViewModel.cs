/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

using System;
using System.Windows.Input;

using CNCLib.WpfClient.Helpers;

using Framework.Wpf.Helpers;

namespace CNCLib.WpfClient.ViewModels.ManualControl
{
    public class RotateViewModel : DetailViewModel
    {
        private readonly Global _global;

        public RotateViewModel(IManualControlViewModel vm, Global global) : base(vm, global)
        {
            _global = global;
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