using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Shos.Chatter.Server.Hubs
{
    public class ChatterHub : Hub
    {
        public async Task UpdateUsers() => await Clients.All.SendAsync("UpdateUsers");
        public async Task UpdateChats() => await Clients.All.SendAsync("UpdateChats");
    }
}