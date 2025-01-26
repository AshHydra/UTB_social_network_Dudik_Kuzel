using Microsoft.AspNetCore.Identity;
using System;

namespace Utb_sc_Domain.Entities
{
    public class Notification : Entity<int>
    {
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        // Relationships
        public int UserId { get; set; }
        public IdentityUser<int> User { get; set; } // Reference AspNetUsers
    }
}
