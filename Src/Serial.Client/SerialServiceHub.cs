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

using System.Threading;
using System.Threading.Tasks;

using Framework.Arduino.SerialCommunication.Abstraction;

using Microsoft.AspNetCore.SignalR.Client;

namespace CNCLib.Serial.Client
{
    public class SerialServiceHub : MyServiceBase
    {
        public SerialServiceHub(string adr, ISerial serial)
        {
            WebServerUri = adr + "/serialSignalR";
            _serial      = serial;
        }

        HubConnection           _connection;
        CancellationTokenSource _cts;
        ISerial                 _serial;

        public async Task Stop()
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        public async Task<HubConnection> Start()
        {
            _connection = new HubConnectionBuilder().WithUrl(WebServerUri)

//            .WithConsoleLogger()
//            .WithMessagePackProtocol()
                //           .WithTransport(TransportType.All)
                .Build();

            await _connection.StartAsync();
            _cts = new CancellationTokenSource();

            _connection.Closed += e =>
            {
                _cts.Cancel();
                return Task.CompletedTask;
            };

            return _connection;
        }
    }
}