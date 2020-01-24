using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace NL.Serverless.AspNetCore.WebApp.Hubs
{
    public class TestHub : Hub
    {
        public async Task SendMessage(string user,string message) 
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
