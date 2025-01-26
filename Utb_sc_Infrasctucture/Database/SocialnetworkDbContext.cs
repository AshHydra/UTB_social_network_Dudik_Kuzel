using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Utb_sc_Domain.Entities;

namespace Utb_sc_Infrastructure.Database
{
    public class SocialNetworkDbContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<FriendList> FriendLists { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ChatUser relationships
            modelBuilder.Entity<ChatUser>()
                .HasKey(cu => new { cu.ChatsId, cu.ParticipantsId });

            modelBuilder.Entity<ChatUser>()
                .HasOne(cu => cu.Chat)
                .WithMany(c => c.ChatUsers)
                .HasForeignKey(cu => cu.ChatsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatUser>()
                .HasOne<IdentityUser<int>>(cu => cu.Participant) // Explicitly reference AspNetUsers
                .WithMany()
                .HasForeignKey(cu => cu.ParticipantsId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure FriendList relationships
            modelBuilder.Entity<FriendList>()
                .HasOne<IdentityUser<int>>(fl => fl.User) // Explicitly reference AspNetUsers
                .WithMany()
                .HasForeignKey(fl => fl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendList>()
                .HasOne<IdentityUser<int>>(fl => fl.Friend) // Explicitly reference AspNetUsers
                .WithMany()
                .HasForeignKey(fl => fl.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Message relationships
            modelBuilder.Entity<Message>()
                .HasOne<IdentityUser<int>>(m => m.Sender) // Explicitly reference AspNetUsers
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Notification relationships
            modelBuilder.Entity<Notification>()
                .HasOne<IdentityUser<int>>(n => n.User) // Explicitly reference AspNetUsers
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed roles and an admin user
            modelBuilder.Entity<IdentityRole<int>>().HasData(new IdentityRole<int>
            {
                Id = 1,
                Name = "Admin",
                NormalizedName = "ADMIN"
            });

            modelBuilder.Entity<IdentityUser<int>>().HasData(new IdentityUser<int>
            {
                Id = 1,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<IdentityUser<int>>().HashPassword(null, "Admin@123"),
                SecurityStamp = Guid.NewGuid().ToString()
            });

            modelBuilder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int>
            {
                UserId = 1,
                RoleId = 1
            });
        }
    }
}
