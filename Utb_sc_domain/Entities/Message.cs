using Utb_sc_Domain.Entities.Interfaces;

namespace Utb_sc_Domain.Entities
{
    public class Message : Entity<int>
    {
        public string Content { get; set; }
        public DateTime SentAt { get; set; }

        public int SenderId { get; set; }
        public IUser<int> Sender { get; set; } // Reference to IUser<int>

        public int ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
