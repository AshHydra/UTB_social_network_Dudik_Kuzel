using Microsoft.EntityFrameworkCore;
using System;
using Utb_sc_Domain.Entities; // Přidejte správný obor názvů pro entity (příklad)

namespace Utb_sc_Infrastructure.Database
{
    public class Social_network_DbContext : DbContext
    {
        public Social_network_DbContext(DbContextOptions<Social_network_DbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        // Další entity

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Konfigurace entit a vztahů
        }
    }
}
