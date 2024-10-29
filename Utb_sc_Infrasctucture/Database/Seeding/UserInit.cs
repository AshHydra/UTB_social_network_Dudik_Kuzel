using Utb_sc_Infrastructure.Identity;

namespace Utb_sc_Infrastructure.Database.Seeding
{
    internal class UserInit
    {
        public User GetAdmin()
        {
            var admin = new User
            {
                Id = 1,
                UserName = "admin",
                Email = "admin@example.com",
                PasswordHash = "AQAAAAEAACcQAAAAEM9O98Suoh2o2JOK1ZOJScgOfQ21odn/k6EYUpGWnrbevCaBFFXrNL7JZxHNczhh/w==", // Heslo předem vygenerované
                NormalizedUserName = "ADMIN",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = "SEJEPXC646ZBNCDYSM3H5FRK5RWP2TN6",
                ConcurrencyStamp = "b09a83ae-cfd3-4ee7-97e6-fbcf0b0fe78c"
            };

            return admin;
        }

        public User GetStandardUser()
        {
            var user = new User
            {
                Id = 2,
                UserName = "user",
                Email = "user@example.com",
                PasswordHash = "AQAAAAEAACcQAAAAEOzeajp5etRMZn7TWj9lhDMJ2GSNTtljLWVIWivadWXNMz8hj6mZ9iDR+alfEUHEMQ==", // Heslo předem vygenerované
                NormalizedUserName = "USER",
                NormalizedEmail = "USER@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = "MAJXOSATJKOEM4YFF32Y5G2XPR5OFEL6",
                ConcurrencyStamp = "7a8d96fd-5918-441b-b800-cbafa99de97b"
            };

            return user;
        }

        public User GetModerator()
        {
            var moderator = new User
            {
                Id = 3,
                UserName = "moderator",
                Email = "moderator@example.com",
                PasswordHash = "AQAAAAEAACcQAAAAEOzeajp5etRMZn7TWj9lhDMJ2GSNTtljLWVIWivadWXNMz8hj6mZ9iDR+alfEUHEMQ==", // Heslo předem vygenerované
                NormalizedUserName = "MODERATOR",
                NormalizedEmail = "MODERATOR@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = "MODERATORSECURITYSTAMP",
                ConcurrencyStamp = "moderator-concurrency-stamp"
            };

            return moderator;
        }
    }
}
