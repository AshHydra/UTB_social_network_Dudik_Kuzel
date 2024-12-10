using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Utb_sc_Domain.Entities;
using Utb_sc_Infrastructure.Identity;
using Utb_sc_Infrastructure.Database.Seeding;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using IdentityUser = Utb_sc_Infrastructure.Identity.User;
using DomainUser = Utb_sc_Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Utb_sc_Infrastructure.Database
{
    public class SocialNetworkDbContext : IdentityDbContext<IdentityUser, Role, int>
    {
        // Definice DbSet pro entity aplikace
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<FriendList> FriendLists { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<IdentityUser> Users { get; set; } // Použití aliasu IdentityUser

        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeding dat pro role
            RolesInit rolesInit = new RolesInit();
            modelBuilder.Entity<Role>().HasData(rolesInit.GetRoles());

            // Seeding dat pro uživatele
            UserInit userInit = new UserInit();
            modelBuilder.Entity<IdentityUser>().HasData(
                userInit.GetAdmin(),
                userInit.GetStandardUser(),
                userInit.GetModerator()
            );

            // Seeding dat pro přiřazení rolí k uživatelům
            UserRolesInit userRolesInit = new UserRolesInit();
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(
                userRolesInit.GetUserRoles()
            );

            // Konfigurace vztahů mezi entitami
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
