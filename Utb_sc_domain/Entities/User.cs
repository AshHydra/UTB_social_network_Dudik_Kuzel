using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;


namespace Utb_sc_Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public string ProfilePictureUrl { get; set; }
        //vztahy
        public ICollection<Message> MessagesSent { get; set; } = new List<Message>();
        public ICollection<Chat> Chats { get; set; } = new List<Chat>();
        public ICollection<FriendList> Friends { get; set; } = new List<FriendList>();
        public ICollection<FriendList> FriendOf { get; set; } = new List<FriendList>();
    }
}
