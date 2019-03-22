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