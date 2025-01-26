using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Utb_sc_Domain.Entities
{
    public class ChatUser
    {
        public int ChatsId { get; set; } // FK to Chats
        public Chat Chat { get; set; }

        public int ParticipantsId { get; set; } // FK to AspNetUsers
        [ForeignKey("ParticipantsId")]
        public IdentityUser<int> Participant { get; set; }
    }
}
