////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Arduino.SerialCommunication;

namespace CNCLib.Wpf.Helpers
{
    public class SerialProxy
    {
        private Framework.Arduino.SerialCommunication.ISerial Com => Framework.Tools.Pattern.Singleton<CNCLib.Serial.Client.SerialService>.Instance;
        private Framework.Arduino.SerialCommunication.ISerial CurrentCom => Com;

        public bool IsConnected => CurrentCom.IsConnected;

        public bool Pause
        {
            get => CurrentCom.Pause;
            set => CurrentCom.Pause = value;
        }
        public bool SendNext
        {
            get => CurrentCom.SendNext;
            set => CurrentCom.SendNext = value;
        }
        public bool ResetOnConnect
        {
            get => CurrentCom.ResetOnConnect;
            set => CurrentCom.ResetOnConnect = value;
        }
        public bool CommandToUpper
        {
            get => CurrentCom.CommandToUpper;
            set => CurrentCom.CommandToUpper = value;
        }
        public int BaudRate
        {
            get => CurrentCom.BaudRate;
            set => CurrentCom.BaudRate = value;
        }
        public int ArduinoBuffersize
        {
            get => CurrentCom.ArduinoBuffersize;
            set => CurrentCom.ArduinoBuffersize = value;
        }

        public void Disconnect() => CurrentCom.Disconnect();


        public IEnumerable<SerialCommand> SendCommand(string line) => CurrentCom.SendCommand(line);
        public async Task<IEnumerable<SerialCommand>> SendCommandAsync(string line, int waitForMilliseconds = int.MaxValue) => await CurrentCom.SendCommandAsync(line, waitForMilliseconds);
        public async Task<IEnumerable<SerialCommand>> SendCommandsAsync(IEnumerable<string> commands) => await CurrentCom.SendCommandsAsync(commands);
        public async Task<string> SendCommandAndReadOKReplyAsync(string line, int waitForMilliseconds = Int32.MaxValue) => await CurrentCom.SendCommandAndReadOKReplyAsync(line, waitForMilliseconds);
        public async Task<IEnumerable<SerialCommand>> SendFileAsync(string filename) => await CurrentCom.SendFileAsync(filename);
        public IEnumerable<SerialCommand> QueueCommand(string line) => CurrentCom.QueueCommand(line);
        public async Task<string> WaitUntilResponseAsync(int waitForMilliseconds = Int32.MaxValue) => await CurrentCom.WaitUntilResponseAsync(waitForMilliseconds);

        public void AbortCommands() => CurrentCom.AbortCommands();
        public void ResumeAfterAbort() => CurrentCom.ResumeAfterAbort();

        public void WritePendingCommandsToFile(string filename) => CurrentCom.WritePendingCommandsToFile(filename);
        public void WriteCommandHistory(string filename) => CurrentCom.WriteCommandHistory(filename);
        public void ClearCommandHistory() => CurrentCom.ClearCommandHistory();
    }
}


