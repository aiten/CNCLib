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
    using System.Collections.Generic;

    using Logging.Abstraction;

    public class HPGLSerial : Serial
    {
        readonly int maxMessageLength = 128;

        public HPGLSerial(ILogger<Serial> logger) : base(logger)
        {
        }

        #region Overrrids

        protected override string[] SplitCommand(string line)
        {
            return SplitHPGL(line);
        }

        protected string[] SplitHPGL(string line)
        {
            string[] cmds    = line.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var      cmdList = new List<string>();

            foreach (string l in cmds)
            {
                string message = l;
                while (message.Length > maxMessageLength)
                {
                    string cmd = message.Substring(0, 2);
                    int    idx = 0;
                    while (idx < maxMessageLength && idx != -1)
                    {
                        idx = message.IndexOf(',', idx + 1);
                        idx = message.IndexOf(',', idx + 1);
                    }

                    if (idx == -1)
                    {
                        break;
                    }

                    string sendMessage = message.Substring(0, idx);
                    message = cmd + message.Substring(idx + 1);
                    cmdList.Add(sendMessage);
                }

                cmdList.Add(message);
            }

            return cmdList.ToArray();
        }

        #endregion
    }
}