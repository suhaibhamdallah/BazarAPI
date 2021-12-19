using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CatalogServer.Services
{
    public class InformHub : Hub
    {
        public async Task SendMessage(string msg)
        {
            await Clients.All.SendAsync("ReceiveMessage", msg);
        }
    }
}