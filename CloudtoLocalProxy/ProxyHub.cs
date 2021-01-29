using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CloudtoLocalProxy
{
    public class ProxyHub : Hub
    {
        public async Task SendMessage(string message)
        {
            System.Console.WriteLine("SendMessage Called");
            await Clients.All.SendCoreAsync("Receive", new object[] { message });
        }

    }

}