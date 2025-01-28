using System.Collections.Generic;

namespace Utb_sc_Domain.Entities
{
    public class Chat : Entity<int>
    {
        public string Name { get; set; }
        public bool IsGroupChat { get; set; }

        // Use ChatUsers for participants
        public ICollection<ChatUsers> Participants { get; set; } = new List<ChatUsers>();

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
