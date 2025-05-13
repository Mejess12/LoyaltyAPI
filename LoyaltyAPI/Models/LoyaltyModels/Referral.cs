using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyAPI.Models
{
    [Index(nameof(ReferralCode), IsUnique = true)]
    public class Referral
    {
        [Key]
        [Column("referral_id")]
        public int ReferralId { get; set; }

        [Required]
        [Column("referral_code")]
        public string ReferralCode { get; set; } = string.Empty;

        [Required]
        [Column("client_id")]
        public int ClientId { get; set; }

        [Column("points")]
        public int Points { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("br_id")]
        public int BrId { get; set; }
    }
}
