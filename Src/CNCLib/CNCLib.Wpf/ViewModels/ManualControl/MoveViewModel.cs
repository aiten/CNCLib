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

using System.Globalization;

using CNCLib.Logic.Contract.DTO;
using CNCLib.Wpf.Helpers;

namespace CNCLib.Wpf.ViewModels.ManualControl
{
    public class MoveViewModel : DetailViewModel
    {
        private readonly Global _global;

        public MoveViewModel(IManualControlViewModel vm, Global global) : base(vm, global)
        {
            _global = global ?? throw new ArgumentNullException();
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