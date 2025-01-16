using System;

namespace Utb_sc_Domain.Entities
{
    public class FriendList : Entity<int>
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int FriendId { get; set; }
        public User Friend { get; set; }

        public DateTime FriendsSince { get; set; }
        public string Status { get; set; } = "Active"; // Např. "Active", "Blocked", atd.
    }
}
