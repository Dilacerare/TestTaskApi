using Microsoft.AspNetCore.SignalR;
using TestTaskApi.Domain.ViewModels;


namespace TestTaskApi.Domain.Handler
{
    public class MessageHub : Hub
    {
        public async Task SendMessage(MessageViewModel message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }

}
