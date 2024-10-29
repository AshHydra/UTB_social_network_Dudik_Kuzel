using System;
using Utb_sc_Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Utb_sc_Infrastructure.Database.Seeding
{
    internal class UserInit
    {
        public User GetAdmin()
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            var admin = new User
            {
                Id = 1,
                UserName = "admin",
                Email = "admin@example.com",
                CreatedAt = DateTime.UtcNow
            };
            admin.PasswordHash = hasher.HashPassword(admin, "AdminPassword"); // Změňte heslo dle potřeby
            return admin;
        }

        public User GetStandardUser()
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            var user = new User
            {
                Id = 2,
                UserName = "user",
                Email = "user@example.com",
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = hasher.HashPassword(user, "UserPassword"); // Změňte heslo dle potřeby
            return user;
        }

        public User GetModerator()
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            var moderator = new User
            {
                Id = 3,
                UserName = "moderator",
                Email = "moderator@example.com",
                CreatedAt = DateTime.UtcNow
            };
            moderator.PasswordHash = hasher.HashPassword(moderator, "ModeratorPassword"); // Změňte heslo dle potřeby
            return moderator;
        }
    }
}
