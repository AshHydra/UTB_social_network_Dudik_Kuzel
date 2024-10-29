using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utb_sc_Domain.Entities.Interfaces;

namespace Utb_sc_Domain.Entities
{
    public class User : Entity<int>, IUser<int>
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }

        // Vztahy
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<Chat> Chats { get; set; } = new List<Chat>();

        // Vztahy pro seznam přátel
        public ICollection<FriendList> Friends { get; set; } = new List<FriendList>();
        public ICollection<FriendList> FriendOf { get; set; } = new List<FriendList>();
    }
}
