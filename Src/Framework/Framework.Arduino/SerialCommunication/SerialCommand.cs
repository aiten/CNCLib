////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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

    public class SerialCommand
    {
        /// <summary>
        /// Sequence Id (incremented with each command)
        /// </summary>
        public int SeqId { get; set; }

        public DateTime? SentTime    { get; set; }
        public string    CommandText { get; set; }

        public EReplyType ReplyType         { get; set; }
        public DateTime?  ReplyReceivedTime { get; set; }

        public string ResultText { get; set; }

        /// <summary>
        /// Index of array passed by PostCommands or SendCommands resulting in this SerialCommand
        /// </summary>
        public int CommandIndex { get; set; }
    }
}