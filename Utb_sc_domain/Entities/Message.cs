using Microsoft.AspNetCore.Identity;
using System;

namespace Utb_sc_Domain.Entities
{
    public class Message : Entity<int>
    {
        public string Content { get; set; }
        public DateTime SentAt { get; set; }

        // Relationships
        public int SenderId { get; set; }
        public IdentityUser<int> Sender { get; set; } // Reference AspNetUsers
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
