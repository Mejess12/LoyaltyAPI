using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoyaltyAPI.Models.WalletModels
{
    [Table("tblLoanApplication")]
    public class LoanApplication
    {
        [Key]
        public int LoanApplicationId { get; set; }

        [Required]
        public int ClientId { get; set; }

        public string LoanNumber { get; set; } = string.Empty;
        public decimal LoanAmount { get; set; }
        public DateTime ApplicationDate { get; set; }

    }
}