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
    internal class SerialPortLib : SerialPortLib2.SerialPortInput, ISerialPort
    {
        public string[] GetPortNames()
        {
            return GetPorts();
        }
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public Parity Parity { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }
        public Handshake Handshake { get; set; }
        public string NewLine { get; set; } = "\r\n";
        public bool DtrEnable { get; set; }

        public bool IsOpen { get => base.IsConnected; }

        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }

        public Stream BaseStream { get => Stream; } 

        public Encoding Encoding { get; } = new ASCIIEncoding();

        public void Close()
        {
            Disconnect();
        }

        public void DiscardOutBuffer()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public void Open()
        {
            SetPort(PortName, BaudRate, (SerialPortLib2.Port.Handshake) Handshake);
            Connect();
            BaseStream.ReadTimeout = ReadTimeout;
        }

        public void WriteLine(string msg)
        {
            throw new NotImplementedException();
        }
    }
}
