using Microsoft.AspNetCore.Identity;

namespace Utb_sc_Infrastructure.Identity
{
    /// <summary>
    /// Custom Role class that can be modified
    /// </summary>
    public class Role : IdentityRole<int>
    {
        public Role(string role) : base(role)
        {
        }

        public Role() : base()
        {
        }
    }
}