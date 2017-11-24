////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

namespace Framework.Arduino.SerialCommunication
{
    public delegate void CommandEventHandler(object sender, SerialEventArgs e);

    [Flags]
    public enum EReplyType
    {
        NoReply = 0,              // no reply received (other options must not be set)
        ReplyOK = 1,
        ReplyError = 2,
        ReplyInfo = 4,
        ReplyUnkown = 8
    }

}
