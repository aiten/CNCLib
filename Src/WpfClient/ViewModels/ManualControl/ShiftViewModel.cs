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

using Framework.Arduino.SerialCommunication;
using Framework.Wpf.Helpers;

namespace CNCLib.WpfClient.ViewModels.ManualControl
{
    public class ShiftViewModel : DetailViewModel
    {
        private readonly Global _global;

        public ShiftViewModel(IManualControlViewModel vm, Global global) : base(vm, global)
        {
            _global = global ?? throw new ArgumentNullException(nameof(global));
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