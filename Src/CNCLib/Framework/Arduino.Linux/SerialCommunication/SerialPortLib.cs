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

namespace Framework.Arduino.Linux.SerialCommunication
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    using Framework.Arduino.SerialCommunication.Abstraction;

    /// <summary>
    /// Implementation for ISerialPort for dependency injection 
    /// </summary>
    public class SerialPortLib : RJCP.IO.Ports.SerialPortStream, ISerialPort
    {
        public SerialPortLib()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                DtrEnable = false;
            }
        }

        public new Parity Parity
        {
            get => (Parity)base.Parity;
            set => base.Parity = ConvertTo(value);
        }

        public new StopBits StopBits
        {
            get => ConvertTo(base.StopBits);
            set => base.StopBits = ConvertTo(value);
        }

        public new Handshake Handshake
        {
            get => (Handshake)base.Handshake;
            set => base.Handshake = ConvertTo(value);
        }

        private RJCP.IO.Ports.Parity ConvertTo(Parity pt)
        {
            switch (pt)
            {
                case Parity.None:  return RJCP.IO.Ports.Parity.None;
                case Parity.Odd:   return RJCP.IO.Ports.Parity.Odd;
                case Parity.Even:  return RJCP.IO.Ports.Parity.Even;
                case Parity.Mark:  return RJCP.IO.Ports.Parity.Mark;
                case Parity.Space: return RJCP.IO.Ports.Parity.Space;
            }

            throw new ArgumentException();
        }

        private RJCP.IO.Ports.Handshake ConvertTo(Handshake hs)
        {
            switch (hs)
            {
                case Handshake.None:                 return RJCP.IO.Ports.Handshake.None;
                case Handshake.XOnXOff:              return RJCP.IO.Ports.Handshake.XOn;
                case Handshake.RequestToSend:        return RJCP.IO.Ports.Handshake.Rts;
                case Handshake.RequestToSendXOnXOff: return RJCP.IO.Ports.Handshake.RtsXOn;
            }

            throw new ArgumentException();
        }

        private RJCP.IO.Ports.StopBits ConvertTo(StopBits sb)
        {
            switch (sb)
            {
                case StopBits.One:          return RJCP.IO.Ports.StopBits.One;
                case StopBits.Two:          return RJCP.IO.Ports.StopBits.Two;
                case StopBits.OnePointFive: return RJCP.IO.Ports.StopBits.One5;
            }

            throw new ArgumentException();
        }

        private StopBits ConvertTo(RJCP.IO.Ports.StopBits sb)
        {
            switch (sb)
            {
                case RJCP.IO.Ports.StopBits.One:  return StopBits.One;
                case RJCP.IO.Ports.StopBits.Two:  return StopBits.Two;
                case RJCP.IO.Ports.StopBits.One5: return StopBits.OnePointFive;
            }

            throw new ArgumentException();
        }

        public new string[] GetPortNames()
        {
            return RJCP.IO.Ports.SerialPortStream.GetPortNames();
        }

        public Stream BaseStream => this;
    }
}