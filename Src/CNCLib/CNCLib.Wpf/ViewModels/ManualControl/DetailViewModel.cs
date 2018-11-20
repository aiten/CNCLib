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

using System;

using CNCLib.Logic.Contract.DTO;

using Framework.Wpf.ViewModels;

namespace CNCLib.Wpf.ViewModels.ManualControl
{
    public class DetailViewModel : BaseViewModel
    {
        protected IManualControlViewModel Vm { get; set; }

        public DetailViewModel(IManualControlViewModel vm)
        {
            Vm = vm;
        }

        public bool Connected => Global.Instance.Com.Current.IsConnected;

        protected void RunInNewTask(Action todo)
        {
            Vm.RunInNewTask(todo);
        }

        protected void RunAndUpdate(Action todo)
        {
            Vm.RunAndUpdate(todo);
        }

        protected void SetPositions(decimal[] positions, int positionIdx)
        {
            Vm.SetPositions(positions, positionIdx);
        }

        #region Command/CanCommand

        public bool CanSend()
        {
            return Connected;
        }

        public bool CanSendGCode()
        {
            return CanSend() && Global.Instance.Machine.CommandSyntax != CommandSyntax.HPGL;
        }

        #endregion
    }
}