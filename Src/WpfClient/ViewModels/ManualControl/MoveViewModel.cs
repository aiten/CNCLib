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

using System.Globalization;
using System.Windows.Input;

using CNCLib.Logic.Abstraction.DTO;
using CNCLib.WpfClient.Helpers;

using Framework.Arduino.SerialCommunication;
using Framework.Wpf.Helpers;

namespace CNCLib.WpfClient.ViewModels.ManualControl
{
    public class MoveViewModel : DetailViewModel
    {
        private readonly Global _global;

        public MoveViewModel(IManualControlViewModel vm, Global global) : base(vm, global)
        {
            _global = global;
        }

        #region Properties

        #endregion

        #region Commands / CanCommands

        private void SendMoveCommand(double? dist, char axisName)
        {
            RunAndUpdate(
                () =>
                {
                    bool   mustUse2Lines = _global.Machine.CommandSyntax == CommandSyntax.Grbl;
                    string commandStr    = _global.Machine.PrepareCommand("g91 g0" + axisName + (dist ?? 0.0).ToString(CultureInfo.InvariantCulture));

                    if (!mustUse2Lines)
                    {
                        commandStr += " g90";
                    }

                    _global.Com.Current.QueueCommand(commandStr);

                    if (mustUse2Lines)
                    {
                        _global.Com.Current.QueueCommand("g90");
                    }
                });
        }

        public bool CanSendCommand(double? dist)
        {
            return CanSend();
        }

        #endregion

        #region ICommands

        public ICommand SendRightCommand =>
            new DelegateCommand<double?>(dist => SendMoveCommand(dist, 'X'), CanSendCommand);

        public ICommand SendLeftCommand =>
            new DelegateCommand<double?>(dist => SendMoveCommand(-dist, 'X'), CanSendCommand);

        public ICommand SendUpCommand =>
            new DelegateCommand<double?>(dist => SendMoveCommand(dist, 'Y'), CanSendCommand);

        public ICommand SendDownCommand =>
            new DelegateCommand<double?>(dist => SendMoveCommand(-dist, 'Y'), CanSendCommand);

        public ICommand SendZUpCommand =>
            new DelegateCommand<double?>(dist => SendMoveCommand(dist, 'Z'), CanSendCommand);

        public ICommand SendZDownCommand =>
            new DelegateCommand<double?>(dist => SendMoveCommand(-dist, 'Z'), CanSendCommand);

        #endregion
    }
}