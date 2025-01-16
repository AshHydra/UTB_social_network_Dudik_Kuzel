using System;

namespace Utb_sc_Domain.Entities
{
    public class FriendList : Entity<int>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int FriendId { get; set; } // Nullable FriendId
        public User Friend { get; set; }
        public DateTime FriendsSince { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Active";
    }

}
