using Microsoft.EntityFrameworkCore;

namespace LoyaltyAPI.Models
{
    public class LoyaltyDbContext(DbContextOptions<LoyaltyDbContext> options) : DbContext(options)
    {
        public DbSet<ClientWallet> ClientWallets { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<Referral> Referral { get; set; }
        public DbSet<Tier> Tier { get; set; }
        public DbSet<Activity> Activity { get; set; }
        public DbSet<SystemSource> SystemSource { get; set; }
        public DbSet<LoyaltyData> LoyaltyData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>().ToTable("transaction");
            modelBuilder.Entity<Referral>().ToTable("referral");
            modelBuilder.Entity<Tier>().ToTable("tier");
            modelBuilder.Entity<Activity>().ToTable("activity");
            modelBuilder.Entity<SystemSource>().ToTable("system_source");
            modelBuilder.Entity<LoyaltyData>().ToTable("loyalty_data");
            modelBuilder.Entity<ClientWallet>().ToTable("tblclientwallet");
        }
    }
}