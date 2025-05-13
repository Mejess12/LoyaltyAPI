using Microsoft.EntityFrameworkCore;
using LoyaltyAPI.Models.WalletModels;
namespace LoyaltyAPI.Models
{
    public class WebLoanAppDbContext : DbContext
    {
        public WebLoanAppDbContext(DbContextOptions<WebLoanAppDbContext> options) : base(options) { }

        public DbSet<ClientWallet> ClientWallets { get; set; }
        public DbSet<LoanApplication> LoanApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ClientWallet>().ToTable("tblclientwallet");
            modelBuilder.Entity<LoanApplication>().ToTable("tblLoanApplication");
        }
    }
}