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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Arduino.SerialCommunication;
using Framework.Tools.Helpers;

namespace CNCLib.Serial.Client
{
    public class SerialService : ServiceBase, ISerial
    {
        protected readonly string _api = @"api/SerialPort";

        private static int GetIdFromPortName(string portname)
        {
            string portNo = portname.Remove(0, 3); // remove "com"
            return (int)uint.Parse(portNo);
        }

        public int PortId { get; private set; }

        public class SerialPortDefinition
        {
            public int Id { get; set; }
            public string PortName { get; set; }
            public bool IsConnected { get; set; }
        }

        public class SerialCommands
        {
            public string[] Commands { get; set; }
        }

        public async void Connect(string portname)
        {
            using (HttpClient client = CreateHttpClient())
            {
                int id = GetIdFromPortName(portname);
                HttpResponseMessage response = await client.PostAsJsonAsync($@"{_api}/{id}/connect?baudRate={BaudRate}?resetOnConnect{ResetOnConnect}","x");
                if (response.IsSuccessStatusCode)
                {
                    SerialPortDefinition value = await response.Content.ReadAsAsync<SerialPortDefinition>();
                    IsConnected = true;
                    PortId = id;
                    return;
                }
                throw new Exception("Connect to SerialPort failed");
            }
        }

        public async void Disconnect()
        {
            if (PortId != 0)
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync($@"{_api}/{PortId}/disconnect", "x");
                    if (response.IsSuccessStatusCode)
                    {
                        IsConnected = false;
                        PortId = 0;
                        return;
                    }
                }
            }
            throw new Exception("DisConnect to SerialPort failed");
        }

        public void AbortCommands()
        {
            throw new NotImplementedException();
        }

        public void ResumeAfterAbort()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SerialCommand> SendCommand(string line)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SerialCommand> QueueCommand(string line)
        {
            if (PortId != 0)
            {
                using (HttpClient client = CreateHttpClient())
                {
                    var cmds = new SerialCommands() { Commands = new string[] { line } };
                    HttpResponseMessage response = client.PostAsJsonAsync($@"{_api}/{PortId}/queue", cmds).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var value = response.Content.ReadAsAsync<IEnumerable<SerialCommand>>().Result;
                        return value;
                    }
                }
            }
            throw new Exception("DisConnect to SerialPort failed");
        }

        public async Task<IEnumerable<SerialCommand>> SendCommandAsync(string line, int waitForMilliseconds = Int32.MaxValue)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SendCommandAndReadOKReplyAsync(string line, int waitForMilliseconds = Int32.MaxValue)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SerialCommand>> SendCommandsAsync(IEnumerable<string> commands)
        {
            throw new NotImplementedException();
        }

        public async Task<string> WaitUntilResponseAsync(int maxMilliseconds = Int32.MaxValue)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SerialCommand>> SendFileAsync(string filename)
        {
            throw new NotImplementedException();
        }

        public event CommandEventHandler WaitForSend;
        public event CommandEventHandler CommandSending;
        public event CommandEventHandler CommandSent;
        public event CommandEventHandler WaitCommandSent;
        public event CommandEventHandler ReplyReceived;
        public event CommandEventHandler ReplyOK;
        public event CommandEventHandler ReplyError;
        public event CommandEventHandler ReplyInfo;
        public event CommandEventHandler ReplyUnknown;
        public event CommandEventHandler CommandQueueChanged;
        public event CommandEventHandler CommandQueueEmpty;
        public bool IsConnected { get; private set; }
        public int CommandsInQueue { get; }
        public bool Pause { get; set; }
        public bool SendNext { get; set; }
        public TraceStream Trace { get; } = new TraceStream();
        public int BaudRate { get; set; }
        public bool ResetOnConnect { get; set; }
        public string OkTag { get; set; }
        public string ErrorTag { get; set; }
        public string InfoTag { get; set; }
        public bool CommandToUpper { get; set; }
        public bool ErrorIsReply { get; set; }
        public int MaxCommandHistoryCount { get; set; }
        public int ArduinoBuffersize { get; set; }
        public int ArduinoLineSize { get; set; }
        public SerialCommand LastCommand { get; }
        public void WriteCommandHistory(string filename)
        {
            throw new NotImplementedException();
        }

        public List<SerialCommand> CommandHistoryCopy { get; }
        public void ClearCommandHistory()
        {
            throw new NotImplementedException();
        }

        public void WritePendingCommandsToFile(string filename)
        {
            throw new NotImplementedException();
        }
    }
}
