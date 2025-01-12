using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utb_sc_Domain.Entities
{
    public class Message : Entity<int>
    {
        public string Content { get; set; }
        public DateTime SentAt { get; set; }

        // Vztahy
        public int SenderId { get; set; }
        public User Sender { get; set; }

        public int ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
