using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utb_sc_Domain.Entities.Interfaces
{
    public class FriendRequest : Entity<int>
    {
        public DateTime RequestedAt { get; set; }
        public bool IsAccepted { get; set; }

        // Vztahy
        public int SenderId { get; set; }
        public User Sender { get; set; }

        public int ReceiverId { get; set; }
        public User Receiver { get; set; }
    }
}
