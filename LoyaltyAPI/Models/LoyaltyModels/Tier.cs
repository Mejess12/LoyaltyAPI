using System;
using System.ComponentModel.DataAnnotations;

namespace LoyaltyAPI.Models
{
    public class Tier
    {
        [Key]
        public int tier_id { get; set; }

        [Required]
        public string description { get; set; } = string.Empty;

        [Required]
        public string badge { get; set; } = string.Empty;

        public DateTime date_added { get; set; } = DateTime.UtcNow;

        public DateTime? update_date { get; set; }
        public long min { get; set; }
        public long max { get; set; }
        public string color { get; set; } = string.Empty;
    }
}
