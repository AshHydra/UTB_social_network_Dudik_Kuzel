using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Utb_sc_Domain.Entities
{
    public class FriendList
    {
        public int Id { get; set; }

        public int UserId { get; set; } // FK to AspNetUsers
        [ForeignKey("UserId")]
        public IdentityUser<int> User { get; set; }

        public int FriendId { get; set; } // FK to AspNetUsers
        [ForeignKey("FriendId")]
        public IdentityUser<int> Friend { get; set; }
    }
}
