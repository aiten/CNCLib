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

using System.Threading;
using System.Threading.Tasks;

using Framework.Arduino.SerialCommunication;
using Framework.Arduino.SerialCommunication.Abstraction;

using Microsoft.AspNetCore.SignalR.Client;

namespace CNCLib.Serial.Client
{
    public class SerialServiceHub : ServiceBase
    {
        public SerialServiceHub(string adr, ISerial serial)
        {
            WebServerUrl = adr + "/serialSignalR";
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
            _connection = new HubConnectionBuilder().WithUrl(WebServerUrl)

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