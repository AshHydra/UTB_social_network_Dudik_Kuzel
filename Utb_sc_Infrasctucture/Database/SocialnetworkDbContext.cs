using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Utb_sc_Domain.Entities;
using Utb_sc_Infrastructure.Identity;
using Utb_sc_Infrastructure.Database.Seeding;
using Microsoft.AspNetCore.Identity;

// Aliases for IdentityUser and DomainUser
using IdentityUser = Utb_sc_Infrastructure.Identity.User;
using DomainUser = Utb_sc_Domain.Entities.User;

namespace Utb_sc_Infrastructure.Database
{
    public class SocialNetworkDbContext : IdentityDbContext<IdentityUser, IdentityRole<int>, int>
    {
        // DbSet definitions for custom application entities
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<FriendList> FriendLists { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Pokud používáte ApplicationUser nebo jinou třídu dědící z IdentityUser
            modelBuilder.Entity<IdentityUser>()
                .Property(u => u.ProfilePicturePath)
                .HasDefaultValue("/images/default.png");

            // Explicitně mapujeme IdentityRole<int> na tabulku AspNetRoles
            modelBuilder.Entity<IdentityRole<int>>().ToTable("AspNetRoles");

            // Seedování rolí
            modelBuilder.Entity<IdentityRole<int>>().HasData(new List<IdentityRole<int>>
            {
                new IdentityRole<int> { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<int> { Id = 2, Name = "User", NormalizedName = "USER" },
                new IdentityRole<int> { Id = 3, Name = "Moderator", NormalizedName = "MODERATOR" }
            });

            // Seedování uživatelů
            modelBuilder.Entity<IdentityUser>().HasData(new List<IdentityUser>
            {
                new IdentityUser
                {
                    Id = 1,
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, "Admin@123"),
                    SecurityStamp = Guid.NewGuid().ToString("D")
                }
            });

            // Seedování vztahů mezi uživateli a rolemi
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(new List<IdentityUserRole<int>>
            {
                new IdentityUserRole<int> { UserId = 1, RoleId = 1 } // Admin uživatel přiřazen k roli Admin
            });

            // Konfigurace vztahů mezi entitami pro FriendList
            modelBuilder.Entity<FriendList>()
                .HasOne(fl => fl.User)
                .WithMany(u => u.Friends)
                .HasForeignKey(fl => fl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendList>()
                .HasOne(fl => fl.Friend)
                .WithMany(u => u.FriendOf)
                .HasForeignKey(fl => fl.FriendId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
