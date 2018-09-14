////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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

using System.Windows.Input;
using Framework.Arduino.SerialCommunication;
using Framework.Wpf.Helpers;
using System.Globalization;
using CNCLib.Wpf.Helpers;

namespace CNCLib.Wpf.ViewModels.ManualControl
{
    public class MoveViewModel : DetailViewModel
    {
        public MoveViewModel(IManualControlViewModel vm) : base(vm)
        {
        }

        #region Properties

        #endregion

        #region Commands / CanCommands

        private void SendMoveCommand(double? dist, char axisname)
        {
            RunAndUpdate(() =>
            {
                bool   mustUse2Lines = Global.Instance.Machine.CommandSyntax == Logic.Contracts.DTO.CommandSyntax.Grbl;
                string commandStr    = MachineGCodeHelper.PrepareCommand("g91 g0" + axisname + (dist ?? 0.0).ToString(CultureInfo.InvariantCulture));

                if (!mustUse2Lines)
                {
                    commandStr += " g90";
                }

                Global.Instance.Com.Current.QueueCommand(commandStr);

                if (mustUse2Lines)
                {
                    Global.Instance.Com.Current.QueueCommand("g90");
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