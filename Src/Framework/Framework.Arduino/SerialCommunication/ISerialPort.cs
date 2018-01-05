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
    /// Interface to mock SerialPort
    /// </summary>
    public interface ISerialPort : IDisposable
    {
        void Open();
        void Close();

        void DiscardOutBuffer();
        void WriteLine(string msg);


        string PortName { get; set; }
        int BaudRate { get; set; }
        System.IO.Ports.Parity Parity { get; set; }
        int DataBits { get; set; }
        System.IO.Ports.StopBits StopBits { get; set; }
        System.IO.Ports.Handshake Handshake { get; set; }
        string NewLine { get; set; }

        bool DtrEnable { get; set; }
        bool IsOpen { get; }

        // Set the read/write timeouts
        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }

        Stream BaseStream { get; }

        Encoding Encoding { get; }
    }

    /// <summary>
    /// Implementation for ISerialPort for dependency injection 
    /// </summary>
    internal class SerialPort : System.IO.Ports.SerialPort, ISerialPort
    {
    }
}
