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
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Framework.Arduino.SerialCommunication;
using Framework.Tools.Helpers;
using CNCLib.Serial.Shared;

namespace CNCLib.Serial.Client
{
    public class SerialService : ServiceBase, ISerial
    {
        protected readonly string _api = @"api/SerialPort";

        public int PortId { get; private set; }

        public void Connect(string portname)
        {
            using (HttpClient client = CreateHttpClient())
            {
                // first ge all ports
                HttpResponseMessage responseAll = client.GetAsync($@"{_api}").GetAwaiter().GetResult();
                if (responseAll.IsSuccessStatusCode)
                {
                    IEnumerable<SerialPortDefinition> allPorts = responseAll.Content.ReadAsAsync<IEnumerable<SerialPortDefinition>>().GetAwaiter().GetResult();
                    var port = allPorts.FirstOrDefault((p) => 0==string.Compare(p.PortName,portname,StringComparison.OrdinalIgnoreCase));
                    if (port != null)
                    {
                        HttpResponseMessage response = client.PostAsJsonAsync(
                            $@"{_api}/{port.Id}/connect?baudRate={BaudRate}&resetOnConnect={ResetOnConnect}", "x").GetAwaiter().GetResult();
                        if (response.IsSuccessStatusCode)
                        {
                            SerialPortDefinition value = response.Content.ReadAsAsync<SerialPortDefinition>().GetAwaiter().GetResult();
                            IsConnected = true;
                            PortId = port.Id;
                            return;
                        }
                    }
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
            throw new Exception("Queue to SerialPort failed");
        }

        public async Task<IEnumerable<SerialCommand>> SendCommandAsync(string line, int waitForMilliseconds = Int32.MaxValue)
        {
            if (PortId != 0)
            {
                using (HttpClient client = CreateHttpClient())
                {
                    var cmds = new SerialCommands() { Commands = new string[] { line } };
                    HttpResponseMessage response = await client.PostAsJsonAsync($@"{_api}/{PortId}/send", cmds);
                    if (response.IsSuccessStatusCode)
                    {
                        var value = response.Content.ReadAsAsync<IEnumerable<SerialCommand>>().Result;
                        return value;
                    }
                }
            }
            throw new Exception("Send to SerialPort failed");
        }

        public async Task<string> SendCommandAndReadOKReplyAsync(string line, int waitForMilliseconds = Int32.MaxValue)
        {
            var ret = await SendCommandAsync(line, waitForMilliseconds);
            if (ret.Any())
            {
                var last = ret.Last();
                return last.ReplyType == EReplyType.ReplyOK ? last.ResultText : null;
            }
            throw new Exception("Send to SerialPort failed");
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

        public List<SerialCommand> CommandHistoryCopy
        {
            get
            {
                if (PortId != 0)
                {
                    using (HttpClient client = CreateHttpClient())
                    {
                        HttpResponseMessage response = client.GetAsync($@"{_api}/{PortId}/history").GetAwaiter().GetResult();
                        if (response.IsSuccessStatusCode)
                        {
                            var value = response.Content.ReadAsAsync<List<SerialCommand>>().Result;
                            return value;
                        }
                    }
                }
                throw new Exception("ClearCommandHistory to SerialPort failed");
            }
        }
        public void ClearCommandHistory()
        {
            if (PortId != 0)
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response = client.PostAsJsonAsync($@"{_api}/{PortId}/history/clear", "x").GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        return;
                    }
                }
            }
            throw new Exception("ClearCommandHistory to SerialPort failed");
        }

        public void WritePendingCommandsToFile(string filename)
        {
            throw new NotImplementedException();
        }
    }
}
