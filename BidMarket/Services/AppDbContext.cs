using BidMarket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BidMarket.Services
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Bet> Bets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Lot> Lots { get; set; }
        public DbSet<LotImage> LotImages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<CategoryLot> CategoryLot { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Review>()
            .HasOne(g => g.Sender)
            .WithMany(t => t.SendReviews)
            .OnDelete(DeleteBehavior.NoAction)
            .HasPrincipalKey(t => t.Id);

            modelBuilder.Entity<Review>()
            .HasOne(g => g.Reciever)
            .WithMany(t => t.RecieveReviews)
            .OnDelete(DeleteBehavior.NoAction)
            .HasPrincipalKey(t => t.Id);
        }
    }
}
