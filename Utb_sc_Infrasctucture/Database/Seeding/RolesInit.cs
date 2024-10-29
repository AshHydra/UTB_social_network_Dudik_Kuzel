using Utb_sc_Infrastructure.Identity;

namespace Utb_sc_Infrastructure.Database.Seeding
{
    internal class RolesInit
    {
        public List<Role> GetRoles()
        {
            List<Role> roles = new List<Role>();

            Role roleAdmin = new Role()
            {
                Id = 1,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "9cf14c2c-19e7-40d6-b744-8917505c3106" // Každá role má jedinečný ConcurrencyStamp
            };

            Role roleUser = new Role()
            {
                Id = 2,
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = "be0efcde-9d0a-461d-8eb6-444b043d6660"
            };

            Role roleModerator = new Role()
            {
                Id = 3,
                Name = "Moderator",
                NormalizedName = "MODERATOR",
                ConcurrencyStamp = "29dafca7-cd20-4cd9-a3dd-4779d7bac3ee"
            };

            roles.Add(roleAdmin);
            roles.Add(roleUser);
            roles.Add(roleModerator);

            return roles;
        }
    }
}
