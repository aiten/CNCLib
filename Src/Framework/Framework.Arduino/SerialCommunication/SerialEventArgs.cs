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

namespace Framework.Arduino.SerialCommunication
{
    using System;

    public class SerialEventArgs : EventArgs
    {
        public SerialEventArgs(string info, SerialCommand cmd)
        {
            Command = cmd;
            if (cmd != null && string.IsNullOrEmpty(info))
            {
                Info = cmd.CommandText;
            }
            else
            {
                Info = info;
            }

            if (cmd != null)
            {
                SeqId = cmd.SeqId;
            }
        }

        public SerialEventArgs(SerialCommand cmd)
        {
            Command = cmd;
            if (cmd != null)
            {
                Info  = cmd.CommandText;
                SeqId = cmd.SeqId;
            }
        }

        public SerialEventArgs(int queueLength, SerialCommand cmd)
        {
            QueueLength = queueLength;
            Command     = cmd;
            if (cmd != null)
            {
                Info  = cmd.CommandText;
                SeqId = cmd.SeqId;
            }
        }

        public SerialEventArgs()
        {
        }

        public bool   Continue { get; set; } = false;
        public bool   Abort    { get; set; } = false;
        public string Result   { get; set; }

        public string Info        { get; }
        public int    QueueLength { get; }

        public int SeqId { get; }

        public SerialCommand Command { get; }
    }
}