using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CNCLib.Serial.Server.Hubs
{
    public class CNCLibHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public async Task QueueEmpty(int id)
        {
            await Clients.All.InvokeAsync("queueEmpty", id);
        }
    }
}
