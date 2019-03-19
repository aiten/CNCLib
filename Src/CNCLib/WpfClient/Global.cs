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

using System.ComponentModel;
using System.Runtime.CompilerServices;

using CNCLib.GCode.Commands;
using CNCLib.WpfClient.Helpers;
using CNCLib.WpfClient.Models;

using Framework.Arduino.SerialCommunication;
using Framework.Arduino.SerialCommunication.Abstraction;
using Framework.Logging;
using Framework.Pattern;

using MachineDto = CNCLib.Logic.Abstraction.DTO.Machine;

namespace CNCLib.WpfClient
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

        public SerialProxy Com { get; set; } = new SerialProxy();

        private JoystickArduinoSerialCommunication _joystickSerialCommunication = new JoystickArduinoSerialCommunication(new FactoryCreate<ISerialPort>(() => new SerialPort()), new Logger<Framework.Arduino.SerialCommunication.Serial>());

        public ISerial ComJoystick => _joystickSerialCommunication;

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