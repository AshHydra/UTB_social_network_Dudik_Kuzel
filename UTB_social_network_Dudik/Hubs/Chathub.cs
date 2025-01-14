using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace UTB_social_network_Dudik.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string chatName, string sender, string message)
        {
            await Clients.Group(chatName).SendAsync("ReceiveMessage", chatName, sender, message);
        }


        public async Task JoinChat(string chatName)
        {
            Console.WriteLine($"User {Context.ConnectionId} joined chat {chatName}");
            await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
        }

        public async Task LeaveChat(string chatName)
        {
            Console.WriteLine($"User {Context.ConnectionId} left chat {chatName}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatName);
        }
    }
}
