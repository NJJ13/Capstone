using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace FreshAir.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            string timeSent = DateTime.Now.ToShortDateString();
            await Clients.All.SendAsync("ReceiveMessage", user, message, timeSent);
        }
    }
}
