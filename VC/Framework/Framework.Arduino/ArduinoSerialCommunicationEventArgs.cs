////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

namespace Framework.Arduino
{
	public class ArduinoSerialCommunicationEventArgs : EventArgs
	{
		public ArduinoSerialCommunicationEventArgs(string info, ArduinoSerialCommunication.Command cmd)
		{
			Command = cmd;
			if (cmd != null && string.IsNullOrEmpty(info))
				this.Info = cmd.CommandText;
			else
				this.Info = info;
			Continue = false;
			Abort = false;
		}

		public bool Continue { get; set; }
		public bool Abort { get; set; }
		public string Result { get; set; }

		public readonly string Info;

		public ArduinoSerialCommunication.Command Command { get; private set; }
	}
}
