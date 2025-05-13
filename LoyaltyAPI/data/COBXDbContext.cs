using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoyaltyAPI.Models
{
    public class COBXDbContext : DbContext
    {
        public COBXDbContext(DbContextOptions<COBXDbContext> options) : base(options) { }

        public DbSet<tblTransactionDetails> tblTransactionDetails { get; set; }
        public DbSet<tblTransactionType> tblTransactionTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<tblTransactionDetails>()
                .ToTable("tblTransactionDetails")
                .HasKey(t => t.TransDetailID);

            modelBuilder.Entity<tblTransactionType>()
                .ToTable("tblTransactionType")
                .HasKey(t => t.TransTypeID);

            // Example: Foreign key configuration (if ClientID references a Client table)
            // modelBuilder.Entity<tblTransactionDetails>()
            //     .HasOne<Client>()
            //     .WithMany()
            //     .HasForeignKey(t => t.ClientID);
        }
    }

    [Table("tblTransactionDetails")]
    public class tblTransactionDetails
    {
        [Key]
        [Column("TransDetailID")]
        public int TransDetailID { get; set; }

        [Required]
        [Column("TransactionCode")]
        public short TransactionCode { get; set; }

        [Column("ClientID")]
        public long? ClientID { get; set; }

        [Column("Amt")]
        public decimal Amt { get; set; }

        [Column("ReferenceNo")]
        public string? ReferenceNo { get; set; }

        [Column("SLE_Code")]
        public byte? SLE_Code { get; set; }


        [Column("BR_ID")]
        public long BR_ID { get; set; }

        [Column("ClientName")]
        public string? ClientName { get; set; }
        public DateTime SLDate { get; internal set; }

        public tblTransactionDetails() { }

        public tblTransactionDetails(int transactionCode, long? clientId, decimal amt)
        {
            TransactionCode = (short)transactionCode;
            ClientID = clientId;
            Amt = amt;
            ClientName = ClientName;
        }
    }

    [Table("tblTransactionType")]
    public class tblTransactionType
    {
        [Key]
        [Column("TransTypeID")]
        public int TransTypeID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("TransTypeDesc")]
        public string TransTypeDesc { get; set; } = string.Empty;

        [Column("TransTypeCode")]
        public string? TransTypeCode { get; set; }

        [Column("TransTypeModule")]
        public string? TransTypeModule { get; set; }

        [Column("TransTypeMode")]
        public string? TransTypeMode { get; set; }

    }
}
