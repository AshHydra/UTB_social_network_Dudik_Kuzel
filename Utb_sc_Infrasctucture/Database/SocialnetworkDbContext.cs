using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Utb_sc_Domain.Entities;
using Utb_sc_Infrastructure.Identity;

namespace Utb_sc_Infrastructure.Database
{
    public class SocialNetworkDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatUsers> ChatUsers { get; set; }
        public DbSet<FriendList> FriendLists { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Message relationships
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasOne(m => (User)m.Sender) // Cast IUser<int> to User
                    .WithMany()
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure ChatUsers relationships
            modelBuilder.Entity<ChatUsers>(entity =>
            {
                entity.HasKey(cu => new { cu.ChatId, cu.UserId }); // Composite primary key

                entity.HasOne(cu => cu.Chat)
                    .WithMany(c => c.Participants)
                    .HasForeignKey(cu => cu.ChatId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cu => (User)cu.User)
                    .WithMany()
                    .HasForeignKey(cu => cu.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure FriendList relationships
            modelBuilder.Entity<FriendList>(entity =>
            {
                entity.HasOne(fl => (User)fl.User) // Explicit cast to User
                    .WithMany()
                    .HasForeignKey(fl => fl.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(fl => (User)fl.Friend) // Explicit cast to User
                    .WithMany()
                    .HasForeignKey(fl => fl.FriendId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Notification relationships
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => (User)n.User)
                    .WithMany()
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed roles
            modelBuilder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int> { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<int> { Id = 2, Name = "User", NormalizedName = "USER" }
            );
        }
    }
}
