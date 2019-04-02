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
using System.Globalization;
using System.Windows;
using System.Windows.Input;

using CNCLib.WpfClient.Helpers;

using Framework.Arduino.SerialCommunication;
using Framework.Wpf.Helpers;

namespace CNCLib.WpfClient.ViewModels.ManualControl
{
    public class AxisViewModel : DetailViewModel
    {
        private readonly Global _global;

        public AxisViewModel(IManualControlViewModel vm, Global global) : base(vm, global)
        {
            _global = global ?? throw new ArgumentNullException();
            _global.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Machine")
                {
                    MachineChanged();
                }
            };
        }

        #region Properties

        public int     AxisIndex { get; set; }
        public string  AxisName  => _global.Machine.GetAxisName(AxisIndex);
        public decimal Size      => _global.Machine.GetSize(AxisIndex);

        //public decimal ProbeSize { get { return _global.Machine.GetProbeSize(AxisIndex); } }
        public bool HomeIsMax { get; set; }

        private string  _param = "0";
        public  decimal ParamDec => decimal.Parse(Param);

        public string Param
        {
            get => _param;
            set => SetProperty(ref _param, value);
        }

        private string _pos = "";

        public string Pos
        {
            get => _pos;
            set => SetProperty(ref _pos, value);
        }

        private string _relPos = "";

        public string RelPos
        {
            get => _relPos;
            set => SetProperty(ref _relPos, value);
        }

        public bool Enabled => _global.Machine.Axis > AxisIndex && Size > 0m;

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
            RunAndUpdate(() => { _global.Com.Current.PrepareAndQueueCommand(_global.Machine, "g91 g0" + AxisName + dist + " g90"); });
        }

        private void SendProbeCommand(int axisIndex)
        {
            RunAndUpdate(async () => { await _global.Com.Current.SendProbeCommandAsync(_global.Machine, AxisIndex); });
        }

        public void SendRefMove()
        {
            RunAndUpdate(() => { _global.Com.Current.PrepareAndQueueCommand(_global.Machine, "g28 " + AxisName + "0"); });
        }

        public void SendG92()
        {
            RunAndUpdate(() => { _global.Com.Current.QueueCommand("g92 " + AxisName + ParamDec.ToString(CultureInfo.InvariantCulture)); });
        }

        public void SendG31()
        {
            SendProbeCommand(AxisIndex);
        }

        public void SendHome()
        {
            RunAndUpdate(
                () =>
                {
                    if (HomeIsMax)
                    {
                        _global.Com.Current.QueueCommand("g53 g0" + AxisName + "#" + (5161 + AxisIndex).ToString());
                    }
                    else
                    {
                        _global.Com.Current.QueueCommand("g53 g0" + AxisName + "0");
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

        public ICommand SendG92Command => new DelegateCommand(
            SendG92,
            () =>
            {
                decimal dummy;
                return CanSendCommandPlotter() && decimal.TryParse(Param, out dummy);
            });

        public ICommand SendG31Command  => new DelegateCommand(SendG31,  CanSendCommandPlotter);
        public ICommand SendHomeCommand => new DelegateCommand(SendHome, CanSendCommandPlotter);

        #endregion
    }
}