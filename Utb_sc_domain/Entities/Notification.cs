using System;
using Utb_sc_Domain.Entities.Interfaces;

namespace Utb_sc_Domain.Entities
{
    public class Notification : Entity<int>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
        public IUser<int> User { get; set; } // Reference to IUser<int>
    }
}
