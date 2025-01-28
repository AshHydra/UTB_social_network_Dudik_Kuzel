using Utb_sc_Domain.Entities.Interfaces;

namespace Utb_sc_Domain.Entities
{
    public class FriendList : Entity<int>
    {
        public int UserId { get; set; }
        public IUser<int> User { get; set; }

        public int FriendId { get; set; }
        public IUser<int> Friend { get; set; }

        public DateTime FriendsSince { get; set; }
    }
}
