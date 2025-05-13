using Microsoft.EntityFrameworkCore;

namespace LoyaltyAPI.Models
{
    public class NFCContext : DbContext
    {
        public NFCContext(DbContextOptions<NFCContext> options) : base(options) { }

        public DbSet<OfficialCard> OfficialCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OfficialCard>().ToTable("official_card");


            modelBuilder.Entity<OfficialCard>(entity =>
            {
                entity.Property(e => e.CardID)
                    .HasColumnName("CardID");

                entity.Property(e => e.CardRefNo)
                    .HasColumnName("CardRefNo");

                entity.Property(e => e.MemberUID)
                    .HasColumnName("MemberUID");

                entity.Property(e => e.CardSerial)
                    .HasColumnName("CardSerial");

                entity.Property(e => e.CardStatus)
                    .HasColumnName("CardStatus");

                entity.Property(e => e.DateAdded)
                    .HasColumnName("DateAdded");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("UpdateDate");

                entity.Property(e => e.AddedBy)
                    .HasColumnName("AddedBy");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("UpdatedBy");

                entity.Property(e => e.CardSerial)
                    .HasColumnName("CardSerial");
            });
        }
    }
}