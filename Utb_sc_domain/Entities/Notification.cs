using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utb_sc_Domain.Entities
{

    public class Notification : Entity<int>
    {   
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        // Vztahy
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
