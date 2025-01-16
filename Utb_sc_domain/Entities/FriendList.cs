using System;

namespace Utb_sc_Domain.Entities
{
    public class FriendList
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? FriendId { get; set; } // Nullable FriendId
        public DateTime FriendsSince { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Active";
    }

}
