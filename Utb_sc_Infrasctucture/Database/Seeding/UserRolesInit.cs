using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Utb_sc_Infrastructure.Database.Seeding
{
    internal class UserRolesInit
    {
        public List<IdentityUserRole<int>> GetRolesForAdmin()
        {
            return new List<IdentityUserRole<int>>
            {
                new IdentityUserRole<int> { UserId = 1, RoleId = 1 } // Admin role pro uživatele s Id = 1
            };
        }

        public List<IdentityUserRole<int>> GetRolesForStandardUser()
        {
            return new List<IdentityUserRole<int>>
            {
                new IdentityUserRole<int> { UserId = 2, RoleId = 2 } // User role pro uživatele s Id = 2
            };
        }

        public List<IdentityUserRole<int>> GetRolesForModerator()
        {
            return new List<IdentityUserRole<int>>
            {
                new IdentityUserRole<int> { UserId = 3, RoleId = 3 } // Moderator role pro uživatele s Id = 3
            };
        }
    }
}
