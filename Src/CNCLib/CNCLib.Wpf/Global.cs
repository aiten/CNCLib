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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using CNCLib.GCode.Commands;
using CNCLib.Wpf.Models;
using CNCLib.Wpf.Helpers;
using Framework.Logging;
using Framework.Tools.Pattern;

using MachineDto = CNCLib.Logic.Contracts.DTO.Machine;

namespace CNCLib.Wpf
{
    public class Global : Singleton<Global>, INotifyPropertyChanged
    {
        private MachineDto _machine = new MachineDto();

        public MachineDto Machine
        {
            get => _machine;
            set
            {
                _machine = value;
                RaisePropertyChanged();
                if (value != null)
                {
                    SizeX = _machine.SizeX;
                    SizeY = _machine.SizeY;
                    SizeZ = _machine.SizeZ;
                }
            }
        }

        private Joystick _joystick;

        public Joystick Joystick
        {
            get => _joystick;
            set
            {
                _joystick = value;
                RaisePropertyChanged();
            }
        }


        private decimal _sizeX = 140m;

        public decimal SizeX
        {
            get => _sizeX;
            set
            {
                _sizeX = value;
                RaisePropertyChanged();
            }
        }

        private decimal _sizeY = 45m;

        public decimal SizeY
        {
            get => _sizeY;
            set
            {
                _sizeY = value;
                RaisePropertyChanged();
            }
        }

        private decimal _sizeZ = 80m;

        public decimal SizeZ
        {
            get => _sizeZ;
            set
            {
                _sizeZ = value;
                RaisePropertyChanged();
            }
        }

        private bool _resetOnConnect = false;

        public bool ResetOnConnect
        {
            get => _resetOnConnect;
            set
            {
                _resetOnConnect = value;
                RaisePropertyChanged();
            }
        }

//        public Framework.Arduino.SerialCommunication.ISerial Com => Framework.Tools.Pattern.Singleton<Framework.Arduino.SerialCommunication.Serial>.Instance;
        public SerialProxy Com { get; set; } = new SerialProxy();

        private JoystickArduinoSerialCommunication _joystickSerialCommunication = new JoystickArduinoSerialCommunication(new Logger<Framework.Arduino.SerialCommunication.Serial>());

        public Framework.Arduino.SerialCommunication.ISerial ComJoystick => _joystickSerialCommunication;

        public MachineGCodeHelper GCode =>
            Framework.Tools.Pattern.Singleton<MachineGCodeHelper>.Instance;

        private CommandList _commands = new CommandList();

        public CommandList Commands
        {
            get => _commands;
            set
            {
                _commands = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}