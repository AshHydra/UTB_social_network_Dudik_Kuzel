using Microsoft.AspNetCore.Identity;
using Utb_sc_Domain.Entities.Interfaces;

namespace Utb_sc_Infrastructure.Identity
{
    /// <summary>
    /// Custom User class that can be modified
    /// </summary>
    public class User : IdentityUser<int>, IUser<int>
    {
        public virtual string? FirstName { get; set; }
        public virtual string? LastName { get; set; }
        public string ProfilePicturePath { get; set; } = "/images/default.png";
    }
}
