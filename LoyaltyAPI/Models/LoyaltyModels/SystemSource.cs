using System.ComponentModel.DataAnnotations;

namespace LoyaltyAPI.Models
{
    public class SystemSource
    {
        [Key]
        public int system_source_id { get; set; }

        [Required]
        public string system_name { get; set; } = string.Empty;

        [Required]
        public string fetch_method { get; set; } = string.Empty;
    }
}
