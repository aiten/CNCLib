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

namespace Framework.Arduino.SerialCommunication.Abstraction
{
    public enum Parity
    {
        None  = 0,
        Odd   = 1,
        Even  = 2,
        Mark  = 3,
        Space = 4
    }

    public enum StopBits
    {
        None         = 0,
        One          = 1,
        Two          = 2,
        OnePointFive = 3
    }

    public enum Handshake
    {
        None                 = 0,
        XOnXOff              = 1,
        RequestToSend        = 2,
        RequestToSendXOnXOff = 3
    }

    /// <summary>
    /// Interface to mock SerialPort
    /// </summary>
    public interface ISerialPort : IDisposable
    {
        string[] GetPortNames();

        void Open();
        void Close();

        void DiscardOutBuffer();
        void WriteLine(string msg);


        string    PortName  { get; set; }
        int       BaudRate  { get; set; }
        Parity    Parity    { get; set; }
        int       DataBits  { get; set; }
        StopBits  StopBits  { get; set; }
        Handshake Handshake { get; set; }
        string    NewLine   { get; set; }

        bool DtrEnable { get; set; }
        bool RtsEnable { get; set; }

        bool IsOpen { get; }

        // Set the read/write timeouts
        int ReadTimeout  { get; set; }
        int WriteTimeout { get; set; }

        Stream BaseStream { get; }

        Encoding Encoding { get; }
    }
}