using Microsoft.AspNetCore.Identity;



namespace Utb_sc_Infrastructure.Database.Seeding
{
    public class UserRolesInit
    {
        public List<IdentityUserRole<int>> GetUserRoles()
        {
            return new List<IdentityUserRole<int>>
            {
                new IdentityUserRole<int> { UserId = 1, RoleId = 1 }, // Admin role for UserId = 1
                new IdentityUserRole<int> { UserId = 2, RoleId = 2 }, // Standard user role for UserId = 2
                new IdentityUserRole<int> { UserId = 3, RoleId = 3 }  // Moderator role for UserId = 3
            };
        }
    }
}
