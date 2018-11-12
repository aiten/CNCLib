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

using System.Globalization;
using System.Windows;
using System.Windows.Input;

using Framework.Arduino.SerialCommunication;
using Framework.Wpf.Helpers;

using CNCLib.Wpf.Helpers;

namespace CNCLib.Wpf.ViewModels.ManualControl
{
    public class AxisViewModel : DetailViewModel
    {
        public AxisViewModel(IManualControlViewModel vm) : base(vm)
        {
            Global.Instance.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Machine")
                {
                    MachineChanged();
                }
            };
        }

        #region Properties

        public int     AxisIndex { get; set; }
        public string  AxisName  => Global.Instance.Machine.GetAxisName(AxisIndex);
        public decimal Size      => Global.Instance.Machine.GetSize(AxisIndex);

        //public decimal ProbeSize { get { return Global.Instance.Machine.GetProbeSize(AxisIndex); } }
        public bool HomeIsMax { get; set; }

        private string  _param = "0";
        public  decimal ParamDec => decimal.Parse(Param);

        public string Param { get => _param; set => SetProperty(ref _param, value); }

        private string _pos = "";

        public string Pos { get => _pos; set => SetProperty(ref _pos, value); }

        private string _relPos = "";

        public string RelPos { get => _relPos; set => SetProperty(ref _relPos, value); }

        public bool Enabled => Global.Instance.Machine.Axis > AxisIndex && Size > 0m;

        public Visibility Visibility => IsDesignTime || Enabled ? Visibility.Visible : Visibility.Hidden;

        private void MachineChanged()
        {
            RaisePropertyChanged(nameof(Enabled));
            RaisePropertyChanged(nameof(Visibility));
        }

        #endregion

        #region Commands / CanCommands

        private void SendMoveCommand(string dist)
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand(MachineGCodeHelper.PrepareCommand("g91 g0" + AxisName + dist + " g90")); });
        }

        private void SendProbeCommand(int axisIndex)
        {
            RunAndUpdate(async () => { await new MachineGCodeHelper().SendProbeCommandAsync(AxisIndex); });
        }

        public void SendRefMove()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand(MachineGCodeHelper.PrepareCommand("g28 " + AxisName + "0")); });
        }

        public void SendG92()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("g92 " + AxisName + ParamDec.ToString(CultureInfo.InvariantCulture)); });
        }

        public void SendG31()
        {
            SendProbeCommand(AxisIndex);
        }

        public void SendHome()
        {
            RunAndUpdate(() =>
            {
                if (HomeIsMax)
                {
                    Global.Instance.Com.Current.QueueCommand("g53 g0" + AxisName + "#" + (5161 + AxisIndex).ToString());
                }
                else
                {
                    Global.Instance.Com.Current.QueueCommand("g53 g0" + AxisName + "0");
                }
            });
        }

        public bool CanSendCommand()
        {
            return CanSend() && Enabled;
        }

        public bool CanSendCommandPlotter()
        {
            return CanSendGCode() && Enabled;
        }

        #endregion

        #region ICommands

        public ICommand SendPlus100Command =>
            new DelegateCommand(() => SendMoveCommand("100"), () => CanSendCommand() && Size >= 100.0m);

        public ICommand SendPlus10Command =>
            new DelegateCommand(() => SendMoveCommand("10"), () => CanSendCommand() && Size >= 10.0m);

        public ICommand SendPlus1Command =>
            new DelegateCommand(() => SendMoveCommand("1"), () => CanSendCommand() && Size >= 1.0m);

        public ICommand SendPlus01Command =>
            new DelegateCommand(() => SendMoveCommand("0.1"), () => CanSendCommand() && Size >= 0.1m);

        public ICommand SendPlus001Command =>
            new DelegateCommand(() => SendMoveCommand("0.01"), () => CanSendCommand() && Size >= 0.01m);

        public ICommand SendMinus100Command =>
            new DelegateCommand(() => SendMoveCommand("-100"), () => CanSendCommand() && Size >= 100.0m);

        public ICommand SendMinus10Command =>
            new DelegateCommand(() => SendMoveCommand("-10"), () => CanSendCommand() && Size >= 10.0m);

        public ICommand SendMinus1Command =>
            new DelegateCommand(() => SendMoveCommand("-1"), () => CanSendCommand() && Size >= 1.0m);

        public ICommand SendMinus01Command =>
            new DelegateCommand(() => SendMoveCommand("-0.1"), () => CanSendCommand() && Size >= 0.1m);

        public ICommand SendMinus001Command =>
            new DelegateCommand(() => SendMoveCommand("-0.01"), () => CanSendCommand() && Size >= 0.01m);

        public ICommand SendRefMoveCommand => new DelegateCommand(SendRefMove, CanSendCommand);

        public ICommand SendG92Command => new DelegateCommand(SendG92, () =>
        {
            decimal dummy;
            return CanSendCommandPlotter() && decimal.TryParse(Param, out dummy);
        });

        public ICommand SendG31Command  => new DelegateCommand(SendG31,  CanSendCommandPlotter);
        public ICommand SendHomeCommand => new DelegateCommand(SendHome, CanSendCommandPlotter);

        #endregion
    }
}