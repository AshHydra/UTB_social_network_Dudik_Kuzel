using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utb_sc_Domain.Entities
{
    public class Chat : Entity<int>
    {
        public string Name { get; set; } // Pro skupinový chat
        public bool IsGroupChat { get; set; }

        // Vztahy
        public ICollection<User> Participants { get; set; } = new List<User>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
