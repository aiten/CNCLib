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
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CNCLib.Serial.Server.Hubs
{
    public class CNCLibHub : Hub
    {
        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task HeartBeat()
        {
            await Clients.All.InvokeAsync("heartbeat");
        }

        public async Task QueueEmpty(int id)
        {
            await Clients.All.InvokeAsync("queueEmpty", id);
        }
        public async Task QueueChanged(int id, int queueLength)
        {
            await Clients.All.InvokeAsync("queueChanged", id, queueLength);
        }

        public async Task Connected(int id)
        {
            await Clients.All.InvokeAsync("connected",id);
        }
        public async Task Disconnected(int id)
        {
            await Clients.All.InvokeAsync("disconnected", id);
        }
    }
}
