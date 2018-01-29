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
        protected readonly string _api = @"api/SerialPort";

        static async Task Run()
        {

            var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/stocks")
            .WithConsoleLogger()
            .WithMessagePackProtocol()
            .WithTransport(TransportType.WebSockets)
            .Build();

            await connection.StartAsync();

            Console.WriteLine("Starting connection. Press Ctrl-C to close.");
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, a) =>
        {
            a.Cancel = true;
            cts.Cancel();
        };

            connection.Closed += e =>
            {
                Console.WriteLine("Connection closed with error: {0}", e);

                cts.Cancel();
                return Task.CompletedTask;
            };

            connection.On("marketOpened", async () =>
            {
                await StartStreaming();
            });

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
        }
    }
}
