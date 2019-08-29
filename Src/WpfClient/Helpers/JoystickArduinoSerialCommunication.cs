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
using System.Threading.Tasks;

using Framework.Arduino.SerialCommunication;
using Framework.Arduino.SerialCommunication.Abstraction;
using Framework.Pattern;

using Microsoft.Extensions.Logging;

using SerialCom = Framework.Arduino.SerialCommunication.Serial;

namespace CNCLib.WpfClient.Helpers
{
    class JoystickArduinoSerialCommunication : SerialCom
    {
        private Global _global;

        public JoystickArduinoSerialCommunication(IFactory<ISerialPort> serialPortFactory, ILogger<SerialCom> logger, Global global) : base(serialPortFactory, logger)
        {
            OkTag   = ""; // every new line is "end of command"
            _global = global;
        }

        public void RunCommandInNewTask(Action todo)
        {
            Task.Run(() => { todo(); });
        }

        protected override void OnReplyReceived(SerialEventArgs info)
        {
            base.OnReplyReceived(info);

            if (info.Info.StartsWith(";CNCJoystick"))
            {
                string initCommands = _global.Joystick?.InitCommands;
                if (initCommands != null)
                {
                    RunCommandInNewTask(
                        async () =>
                        {
                            string[] separators = { @"\n" };
                            string[] cmds       = initCommands.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                            await QueueCommandsAsync(cmds);
                        });
                }
            }
            else
            {
                RunCommandInNewTask(
                    async () =>
                    {
                        string msg = _global.Machine.JoystickReplyReceived(info.Info.Trim());
                        await _global.Com.Current.QueueCommandsAsync(new string[] { msg }).ConfigureAwait(false);
                    });
            }
        }
    }
}