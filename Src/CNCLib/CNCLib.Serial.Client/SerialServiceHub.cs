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
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Sockets;
using System.Threading;

namespace CNCLib.Serial.Client
{
    public class SerialServiceHub : ServiceBase
    {
        public SerialServiceHub(string adr, ISerial serial)
        {
            WebServerUrl = adr + "/serialSignalR";
            _serial = serial;
        }

        HubConnection _connection;
        CancellationTokenSource _cts;
        ISerial _serial;

        public async Task Stop()
        {
            if (_connection!=null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        public async Task<HubConnection> Start()
        {
            _connection = new HubConnectionBuilder()
            .WithUrl(WebServerUrl)
            .WithConsoleLogger()
            .WithMessagePackProtocol()
            .WithTransport(TransportType.WebSockets)
            .Build();

            await _connection.StartAsync();
            _cts = new CancellationTokenSource();
/*
            Console.WriteLine("Starting connection. Press Ctrl-C to close.");
            Console.CancelKeyPress += (sender, a) =>
            {
                a.Cancel = true;
                _cts.Cancel();
            };
*/
            _connection.Closed += e =>
            {
                Console.WriteLine("Connection closed with error: {0}", e);

                _cts.Cancel();
                return Task.CompletedTask;
            };

            _connection.On("queueEmpty", async () =>
            {
                Console.WriteLine("QueueEmpty");
            });
            _connection.On("connected", async () =>
            {
                Console.WriteLine("Connected");
            });

            _connection.On("heartbeat", async () =>
            {
                if (_serial != null)
                {
                    //_serial.CommandQueueEmpty?.Invoke(_serial,new SerialEventArgs("info",null));
                }

                Console.WriteLine("heartbeat");
            });

            _connection.Connected +=
                async () => { Console.WriteLine("Connected"); };

            /*

                            // Do an initial check to see if we can start streaming the stocks
                            var state = await connection.InvokeAsync<string>("GetMarketState");
                            if (string.Equals(state, "Open"))
                            {
                                await StartStreaming();
                            }

                            // Keep client running until cancel requested.
                            while (!cts.IsCancellationRequested)
                            {
                                await Task.Delay(250);
                            }

                            async Task StartStreaming()
                            {
                                var channel = await connection.StreamAsync<Stock>("StreamStocks", CancellationToken.None);
                                while (await channel.WaitToReadAsync() && !cts.IsCancellationRequested)
                                {
                                    while (channel.TryRead(out var stock))
                                    {
                                        Console.WriteLine($"{stock.Symbol} {stock.Price}");
                                    }
                                }
                            }
                        */
            return _connection;
        }
    }
}
