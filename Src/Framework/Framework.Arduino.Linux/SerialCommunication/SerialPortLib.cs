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

namespace Framework.Arduino.Linux.SerialCommunication
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using Framework.Arduino.SerialCommunication;

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

        public new Parity Parity { get => (Parity) base.Parity; set => base.Parity = ConvertTo(value); }

        public new StopBits StopBits { get => ConvertTo(base.StopBits); set => base.StopBits = ConvertTo(value); }

        public new Handshake Handshake { get => (Handshake) base.Handshake; set => base.Handshake = ConvertTo(value); }

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