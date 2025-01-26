using System.Collections.Generic;

namespace Utb_sc_Domain.Entities
{
    public class Chat : Entity<int>
    {
        public string Name { get; set; } // For group chat
        public bool IsGroupChat { get; set; } // Whether the chat is a group chat or not

        // Relationships
        public ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>(); // Many-to-many relationship with users
        public ICollection<Message> Messages { get; set; } = new List<Message>(); // One-to-many relationship with messages
    }
}
