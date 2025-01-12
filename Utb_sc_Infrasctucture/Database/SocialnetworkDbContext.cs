using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Utb_sc_Domain.Entities;
using Utb_sc_Infrastructure.Identity;

// Aliases for IdentityUser and DomainUser
using IdentityUser = Utb_sc_Infrastructure.Identity.User;

namespace Utb_sc_Infrastructure.Database
{
    public class SocialNetworkDbContext : IdentityDbContext<IdentityUser, IdentityRole<int>, int>
    {
        // DbSet definitions for your entities
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<FriendList> FriendLists { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicitly map IdentityRole<int> to AspNetRoles table
            modelBuilder.Entity<IdentityRole<int>>().ToTable("AspNetRoles");

            // Configuring the Message entity relationships
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.Id); // Primary key

                entity.Property(m => m.Content)
                    .IsRequired()
                    .HasMaxLength(500); // Limit content length to 500 characters

                entity.HasOne(m => m.Sender)
                    .WithMany(u => u.MessagesSent)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

                entity.HasOne(m => m.Chat)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(m => m.ChatId)
                    .OnDelete(DeleteBehavior.Cascade); // Allow cascade delete for messages in a chat
            });

            // Configuring other entities (FriendList, Chat, etc.) if necessary
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

            // Other configurations for Identity entities can stay unchanged
        }
    }
}
