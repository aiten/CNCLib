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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Converters;
using CNCLib.GCode;
using CNCLib.Wpf.Helpers;
using Framework.Wpf.Helpers;
using Framework.Arduino.SerialCommunication;

namespace CNCLib.Wpf.ViewModels.ManualControl
{
    public class WorkOffsetViewModel : DetailViewModel
    {
        public WorkOffsetViewModel(IManualControlViewModel vm)
            : base(vm) { }

        #region Properties

        private decimal?[,] _g54 = new decimal?[3, 6];

        private void SetField(int axis, int offset, decimal? val)
        {
            _g54[axis, offset] = val;
            RaisePropertyChanged($"G{54 + offset}{GCodeHelper.IndexToAxisName(axis)}");
        }

        public decimal? G54X
        {
            get => _g54[0, 0];
            set => SetField(0, 0, value);
        }

        public decimal? G54Y
        {
            get => _g54[1, 0];
            set => SetField(1, 0, value);
        }

        public decimal? G54Z
        {
            get => _g54[2, 0];
            set => SetField(2, 0, value);
        }

        public decimal? G55X
        {
            get => _g54[0, 1];
            set => SetField(0, 1, value);
        }

        public decimal? G55Y
        {
            get => _g54[1, 1];
            set => SetField(1, 1, value);
        }

        public decimal? G55Z
        {
            get => _g54[2, 1];
            set => SetField(2, 1, value);
        }

        public decimal? G56X
        {
            get => _g54[0, 2];
            set => SetField(0, 2, value);
        }

        public decimal? G56Y
        {
            get => _g54[1, 2];
            set => SetField(1, 2, value);
        }

        public decimal? G56Z
        {
            get => _g54[2, 2];
            set => SetField(2, 2, value);
        }

        public decimal? G57X
        {
            get => _g54[0, 3];
            set => SetField(0, 3, value);
        }

        public decimal? G57Y
        {
            get => _g54[1, 3];
            set => SetField(1, 3, value);
        }

        public decimal? G57Z
        {
            get => _g54[2, 3];
            set => SetField(2, 3, value);
        }

        public decimal? G58X
        {
            get => _g54[0, 4];
            set => SetField(0, 4, value);
        }

        public decimal? G58Y
        {
            get => _g54[1, 4];
            set => SetField(1, 4, value);
        }

        public decimal? G58Z
        {
            get => _g54[2, 4];
            set => SetField(2, 4, value);
        }

        public decimal? G59X
        {
            get => _g54[0, 5];
            set => SetField(0, 5, value);
        }

        public decimal? G59Y
        {
            get => _g54[1, 5];
            set => SetField(1, 5, value);
        }

        public decimal? G59Z
        {
            get => _g54[2, 5];
            set => SetField(2, 5, value);
        }

        #endregion

        #region Commands / CanCommands

        public void SendG53()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("g53"); });
        }

        public void SendG54()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("g54"); });
        }

        public void SendG55()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("g55"); });
        }

        public void SendG56()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("g56"); });
        }

        public void SendG57()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("g57"); });
        }

        public void SendG58()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("g58"); });
        }

        public void SendG59()
        {
            RunAndUpdate(() => { Global.Instance.Com.Current.QueueCommand("g59"); });
        }

        private async Task<decimal?> GetParameterValue(int parameter)
        {
            string message =
                await Global.Instance.Com.Current
                    .SendCommandAndReadOKReplyAsync(MachineGCodeHelper.PrepareCommand($"(print, #{parameter})"),
                                                    10 * 1000);

            if (!string.IsNullOrEmpty(message))
            {
                // expected response : 0\nok
                string pos = message.Split('\n').FirstOrDefault();
                if (decimal.TryParse(pos, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val))
                {
                    return val / 1000m;
                }
            }

            return null;
        }

        public void GetG5x(int offsetG)
        {
            RunAndUpdate(async () =>
            {
                var gX = await GetParameterValue(5221 + offsetG * 20);
                if (gX.HasValue)
                {
                    SetField(0, offsetG, gX);
                }

                var gY = await GetParameterValue(5222 + offsetG * 20);
                if (gY.HasValue)
                {
                    SetField(1, offsetG, gY);
                }

                var gZ = await GetParameterValue(5223 + offsetG * 20);
                if (gZ.HasValue)
                {
                    SetField(2, offsetG, gZ);
                }
            });
        }

        public bool CanGetG5x(int offsetG)
        {
            return CanSendGCode();
        }

        public void SetG5x(int offsetG)
        {
            string x = _g54[0, offsetG].HasValue
                ? $" X{_g54[0, offsetG].Value.ToString(CultureInfo.InvariantCulture)}"
                : "";
            string y = _g54[1, offsetG].HasValue
                ? $" Y{_g54[1, offsetG].Value.ToString(CultureInfo.InvariantCulture)}"
                : "";
            string z = _g54[2, offsetG].HasValue
                ? $" Z{_g54[2, offsetG].Value.ToString(CultureInfo.InvariantCulture)}"
                : "";

            // p0 => current
            // p1 => g54
            // p2 => g55
            Global.Instance.Com.Current.QueueCommand($"g10 l2 p{offsetG + 1}{x}{y}{z}");
        }

        public bool CanSetG5x(int offsetG)
        {
            return CanSendGCode();
        }

        #endregion

        #region ICommand

        public ICommand SendG53Command => new DelegateCommand(SendG53,         CanSendGCode);
        public ICommand SendG54Command => new DelegateCommand(SendG54,         CanSendGCode);
        public ICommand SendG55Command => new DelegateCommand(SendG55,         CanSendGCode);
        public ICommand SendG56Command => new DelegateCommand(SendG56,         CanSendGCode);
        public ICommand SendG57Command => new DelegateCommand(SendG57,         CanSendGCode);
        public ICommand SendG58Command => new DelegateCommand(SendG58,         CanSendGCode);
        public ICommand SendG59Command => new DelegateCommand(SendG59,         CanSendGCode);
        public ICommand GetG54Command  => new DelegateCommand(() => GetG5x(0), () => CanGetG5x(0));
        public ICommand SetG54Command  => new DelegateCommand(() => SetG5x(0), () => CanSetG5x(0));
        public ICommand GetG55Command  => new DelegateCommand(() => GetG5x(1), () => CanGetG5x(1));
        public ICommand SetG55Command  => new DelegateCommand(() => SetG5x(1), () => CanSetG5x(1));
        public ICommand GetG56Command  => new DelegateCommand(() => GetG5x(2), () => CanGetG5x(2));
        public ICommand SetG56Command  => new DelegateCommand(() => SetG5x(2), () => CanSetG5x(2));
        public ICommand GetG57Command  => new DelegateCommand(() => GetG5x(3), () => CanGetG5x(2));
        public ICommand SetG57Command  => new DelegateCommand(() => SetG5x(3), () => CanSetG5x(2));
        public ICommand GetG58Command  => new DelegateCommand(() => GetG5x(4), () => CanGetG5x(2));
        public ICommand SetG58Command  => new DelegateCommand(() => SetG5x(4), () => CanSetG5x(2));
        public ICommand GetG59Command  => new DelegateCommand(() => GetG5x(5), () => CanGetG5x(2));
        public ICommand SetG59Command  => new DelegateCommand(() => SetG5x(5), () => CanSetG5x(2));

        #endregion
    }
}