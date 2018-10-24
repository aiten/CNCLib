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

using CNCLib.Serial.Shared;

using Framework.Arduino.SerialCommunication;

using Microsoft.AspNetCore.SignalR.Client;

namespace CNCLib.Serial.Client
{
    public class SerialService : ServiceBase, ISerial
    {
        protected readonly string           _api = @"api/SerialPort";
        private            SerialServiceHub _serviceHub;

        private async Task InitServiceHub()
        {
            _serviceHub = new SerialServiceHub(WebServerUrl, this);
            var connection = await _serviceHub.Start();

            connection.On("queueEmpty", (int id) =>
            {
                if (PortId == id)
                {
                    CommandQueueEmpty?.Invoke(this, new SerialEventArgs());
                }
            });
            connection.On("queueChanged", (int id, int queuelength) =>
            {
                if (PortId == id)
                {
                    CommandQueueChanged?.Invoke(this, new SerialEventArgs(queuelength, null));
                }
            });
            connection.On("sendingCommand", (int id, int seqId) =>
            {
                if (PortId == id)
                {
                    CommandSending?.Invoke(this, new SerialEventArgs(new SerialCommand() { SeqId = seqId }));
                }
            });
        }

        public int PortId { get; private set; } = -1;

        private async Task<SerialPortDefinition> GetSerialPortDefinition(HttpClient client, string portname)
        {
            // first ge all ports
            HttpResponseMessage responseAll = client.GetAsync($@"{_api}").GetAwaiter().GetResult();
            if (responseAll.IsSuccessStatusCode)
            {
                IEnumerable<SerialPortDefinition> allPorts = await responseAll.Content.ReadAsAsync<IEnumerable<SerialPortDefinition>>();
                return allPorts.FirstOrDefault((p) => 0 == string.Compare(p.PortName, portname, StringComparison.OrdinalIgnoreCase));
            }

            return null;
        }

        private async Task<SerialPortDefinition> RefreshAndGetSerialPortDefinition(HttpClient client, string portname)
        {
            HttpResponseMessage responseAll = client.PostAsJsonAsync($@"{_api}/refresh", "dummy").GetAwaiter().GetResult();
            if (responseAll.IsSuccessStatusCode)
            {
                IEnumerable<SerialPortDefinition> allPorts = await responseAll.Content.ReadAsAsync<IEnumerable<SerialPortDefinition>>();
                return allPorts.FirstOrDefault((p) => 0 == string.Compare(p.PortName, portname, StringComparison.OrdinalIgnoreCase));
            }

            return null;
        }

        public async Task ConnectAsync(string portname)
        {
            if (WaitForSend != null || CommandSent != null || WaitCommandSent != null || ReplyReceived != null || ReplyOK != null || ReplyError != null || ReplyInfo != null || ReplyUnknown != null)
            {
                // dummy do not get "unused"                
            }

            // linuxport => http://a0:5000/dev/ttyUSB0
            // win       => http://a0:5000/com4

            int lastcolon = portname.LastIndexOf(':');
            int lastslash = portname.IndexOf('/', lastcolon);
            if (lastslash > 0)
            {
                WebServerUrl = portname.Substring(0, lastslash);
                portname     = portname.Substring(lastslash + 1);
                if (portname.IndexOf('/') >= 0)
                {
                    portname = "/" + portname;
                }

                using (HttpClient client = CreateHttpClient())
                {
                    var port = await GetSerialPortDefinition(client, portname) ?? await RefreshAndGetSerialPortDefinition(client, portname);

                    if (port != null)
                    {
                        HttpResponseMessage response = await client.PostAsJsonAsync($@"{_api}/{port.Id}/connect?baudRate={BaudRate}&dtrIsReset={DtrIsReset}&resetOnConnect={ResetOnConnect}", "x");
                        if (response.IsSuccessStatusCode)
                        {
                            var value = await response.Content.ReadAsAsync<SerialPortDefinition>();
                            IsConnected = true;
                            PortId      = port.Id;

                            await InitServiceHub();
                            return;
                        }
                    }
                }
            }

            throw new Exception("Connect to SerialPort failed");
        }

        public async Task DisconnectAsync()
        {
            if (PortId >= 0)
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync($@"{_api}/{PortId}/disconnect", "x");
                    if (response.IsSuccessStatusCode)
                    {
                        _serviceHub?.Stop();
                        _serviceHub = null;
                        IsConnected = false;
                        PortId      = -1;
                        return;
                    }
                }
            }

            throw new Exception("DisConnect to SerialPort failed");
        }

        public void AbortCommands()
        {
            if (PortId >= 0)
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response = client.PostAsJsonAsync($@"{_api}/{PortId}/abort", "x").ConfigureAwait(false).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        return;
                    }
                }
            }

            throw new Exception("AbortCommands to SerialPort failed");
        }

        public void ResumeAfterAbort()
        {
            if (PortId >= 0)
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response = client.PostAsJsonAsync($@"{_api}/{PortId}/resume", "x").ConfigureAwait(false).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        return;
                    }
                }
            }

            throw new Exception("ResumeAfterAbort to SerialPort failed");
        }

        public async Task<IEnumerable<SerialCommand>> QueueCommandsAsync(IEnumerable<string> lines)
        {
            if (PortId >= 0)
            {
                using (HttpClient client = CreateHttpClient())
                {
                    var                 cmds     = new SerialCommands() { Commands = lines.ToArray() };
                    HttpResponseMessage response = client.PostAsJsonAsync($@"{_api}/{PortId}/queue", cmds).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var value = await response.Content.ReadAsAsync<IEnumerable<SerialCommand>>();
                        return value;
                    }
                }
            }

            throw new Exception("Queue to SerialPort failed");
        }

        public async Task<IEnumerable<SerialCommand>> SendCommandsAsync(IEnumerable<string> lines, int waitForMilliseconds)
        {
            if (PortId >= 0)
            {
                using (HttpClient client = CreateHttpClient())
                {
                    client.Timeout = new TimeSpan(10000L * (((long) waitForMilliseconds) + 5000));
                    var                 cmds     = new SerialCommands() { Commands = lines.ToArray(), TimeOut = waitForMilliseconds };
                    HttpResponseMessage response = await client.PostAsJsonAsync($@"{_api}/{PortId}/send", cmds);
                    if (response.IsSuccessStatusCode)
                    {
                        var value = await response.Content.ReadAsAsync<IEnumerable<SerialCommand>>();
                        return value;
                    }
                }
            }

            throw new Exception("Send to SerialPort failed");
        }

        public Task<bool> WaitUntilResponseAsync(int maxMilliseconds)
        {
            throw new NotImplementedException();
        }

        public Task<bool> WaitUntilQueueEmptyAsync(int maxMilliseconds)
        {
            throw new NotImplementedException();
        }

        Task<string> ISerial.WaitUntilResponseAsync(int maxMilliseconds)
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
        public bool                      IsConnected            { get; private set; }
        public int                       CommandsInQueue        { get; }
        public bool                      Pause                  { get; set; }
        public bool                      SendNext               { get; set; }
        public int                       BaudRate               { get; set; }
        public bool                      DtrIsReset             { get; set; }
        public bool                      ResetOnConnect         { get; set; }
        public string                    OkTag                  { get; set; }
        public string                    ErrorTag               { get; set; }
        public string                    InfoTag                { get; set; }
        public bool                      CommandToUpper         { get; set; }
        public bool                      ErrorIsReply           { get; set; }
        public int                       MaxCommandHistoryCount { get; set; }
        public int                       ArduinoBuffersize      { get; set; }
        public int                       ArduinoLineSize        { get; set; }
        public bool                      Aborted                { get; }

        public SerialCommand LastCommand { get; }

        public void WriteCommandHistory(string filename)
        {
            throw new NotImplementedException();
        }

        public List<SerialCommand> CommandHistoryCopy
        {
            get
            {
                if (PortId >= 0)
                {
                    using (HttpClient client = CreateHttpClient())
                    {
                        HttpResponseMessage response = client.GetAsync($@"{_api}/{PortId}/history").ConfigureAwait(false).GetAwaiter().GetResult();
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

        public IEnumerable<SerialCommand> PendingCommands => throw new NotImplementedException();

        public void ClearCommandHistory()
        {
            if (PortId >= 0)
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response = client.PostAsJsonAsync($@"{_api}/{PortId}/history/clear", "x").ConfigureAwait(false).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        return;
                    }
                }
            }

            throw new Exception("ClearCommandHistory to SerialPort failed");
        }
    }
}