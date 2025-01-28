using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Utb_sc_Infrastructure.Database;
using Utb_sc_Domain.Entities;
using Utb_sc_Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class ChatHub : Hub
{
    private readonly SocialNetworkDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public ChatHub(SocialNetworkDbContext dbContext, UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task SendMessage(int chatId, string sender, string message)
    {
        var user = await _userManager.FindByNameAsync(sender);
        if (user == null)
        {
            Console.WriteLine($"❌ User not found: {sender}");
            return;
        }

        Console.WriteLine($"📩 {sender} sending message to Chat {chatId}: {message}");

        var chatMessage = new Message
        {
            ChatId = chatId,
            SenderId = user.Id,
            Content = message,
            SentAt = DateTime.UtcNow
        };

        _dbContext.Messages.Add(chatMessage);
        await _dbContext.SaveChangesAsync();

        Console.WriteLine("✅ Message saved in DB.");

        await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", chatId, sender, message);
    }

    public async Task JoinChat(int chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());

        var messages = await _dbContext.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentAt)
            .Select(m => new
            {
                Sender = _dbContext.Users.Where(u => u.Id == m.SenderId).Select(u => u.UserName).FirstOrDefault(),
                Content = m.Content,
                SentAt = m.SentAt
            })
            .ToListAsync();

        Console.WriteLine($"✅ Loaded {messages.Count} messages for chat {chatId}");

        foreach (var msg in messages)
        {
            Console.WriteLine($"📨 Sending Message: Sender: {msg.Sender}, Content: {msg.Content}, SentAt: {msg.SentAt}");
        }

        await Clients.Caller.SendAsync("LoadChatHistory", chatId, messages);
    }


}
