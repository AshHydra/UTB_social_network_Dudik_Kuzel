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
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == sender);
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
        await _dbContext.SaveChangesAsync(); // Ensure message is saved before sending

        Console.WriteLine("✅ Message saved in DB.");

        string profilePicture = user.ProfilePicturePath ?? "/images/default.png";

        await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", chatId, sender, message, profilePicture);
    }


    public async Task JoinChat(int chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());

        var messages = await _dbContext.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentAt)
            .Select(m => new
            {
                Sender = m.Sender.UserName,
                Content = m.Content,
                SentAt = m.SentAt,
                ProfilePicture = m.Sender.ProfilePicturePath ?? "/images/default.png"
            })
            .ToListAsync();

        Console.WriteLine($"✅ Sending {messages.Count} messages for chat {chatId}");
        foreach (var msg in messages)
        {
            Console.WriteLine($"➡️ Message: Sender={msg.Sender}, Content={msg.Content}, Picture={msg.ProfilePicture}");
        }

        await Clients.Caller.SendAsync("LoadChatHistory", chatId, messages);
    }


}
