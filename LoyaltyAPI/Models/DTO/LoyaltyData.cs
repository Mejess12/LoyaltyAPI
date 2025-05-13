using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoyaltyAPI.Models
{
    [Table("loyalty_data")]
    public class LoyaltyData
    {
        [Key]
        [Column("client_id")]
        public int ClientID { get; set; }

        [Column("points")]
        public int Points { get; set; }

        [Column("tier")]
        public string Tier { get; set; } = string.Empty;
    }
}
