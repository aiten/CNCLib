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

using Microsoft.AspNetCore.SignalR;

namespace CNCLib.WebAPI.Hubs
{
    public class CNCLibHub : Hub
    {
        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task HeartBeat()
        {
            await Clients.All.SendAsync("heartbeat");
        }

        public async Task QueueEmpty(int id)
        {
            await Clients.All.SendAsync("queueEmpty", id);
        }

        public async Task QueueChanged(int id, int queueLength)
        {
            await Clients.All.SendAsync("queueChanged", id, queueLength);
        }

        public async Task SendingCommand(int id, int seqId)
        {
            await Clients.All.SendAsync("sendingCommand", id, seqId);
        }

        public async Task Connected(int id)
        {
            await Clients.All.SendAsync("connected", id);
        }

        public async Task Disconnected(int id)
        {
            await Clients.All.SendAsync("disconnected", id);
        }
    }
}