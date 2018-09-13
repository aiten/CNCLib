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
using System.IO;
using System.Text;

namespace Framework.Arduino.SerialCommunication
{
    /// <summary>
    /// Implementation for ISerialPort for dependency injection 
    /// </summary>
    internal class SerialPort : System.IO.Ports.SerialPort, ISerialPort
    {
        public new string[] GetPortNames()
        {
            return System.IO.Ports.SerialPort.GetPortNames();
        }

        public new Parity Parity
        {
            get => (Parity) base.Parity;
            set => base.Parity = (System.IO.Ports.Parity) value;
        }

        public new StopBits StopBits
        {
            get => (StopBits) base.StopBits;
            set => base.StopBits = (System.IO.Ports.StopBits) value;
        }

        public new Handshake Handshake
        {
            get => (Handshake) base.Handshake;
            set => base.Handshake = (System.IO.Ports.Handshake) value;
        }
    }
}