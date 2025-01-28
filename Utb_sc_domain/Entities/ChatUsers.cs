using Utb_sc_Domain.Entities.Interfaces;

namespace Utb_sc_Domain.Entities
{
    public class ChatUsers : Entity<int>
    {
        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        public int UserId { get; set; }
        public IUser<int> User { get; set; } // Reference to IUser<int>

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
