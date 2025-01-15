using System.Collections.Generic;
using Utb_sc_Infrastructure.Identity;

namespace UTB_social_network_Dudik.Models
{
    public class ContactsViewModel
    {
        public List<User> Contacts { get; set; } = new List<User>();
    }
}
