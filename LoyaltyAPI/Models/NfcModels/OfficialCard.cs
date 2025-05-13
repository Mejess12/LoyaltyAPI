using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoyaltyAPI.Models
{
    public class OfficialCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CardID")]
        public int CardID { get; set; }

        [Required]
        [Column("CardRefNo")]
        public string CardRefNo { get; set; } = null!;

        [Required]
        [Column("MemberUID")]
        public string MemberUID { get; set; } = null!;

        [Column("CardStatus")]
        public int CardStatus { get; set; }

        [Column("DateAdded")]
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        [Column("UpdateDate")]
        public DateTime? UpdateDate { get; set; } = null;

        [Column("AddedBy")]
        public string AddedBy { get; set; } = null!;

        [Column("UpdatedBy")]
        public int? UpdatedBy { get; set; }

        [Column("CardSerial")]
        public string CardSerial { get; set; } = null!;
    }
}