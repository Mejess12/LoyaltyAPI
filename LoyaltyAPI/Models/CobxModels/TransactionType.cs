namespace LoyaltyAPI.Models
{
    public class TransactionType
    {
        public int TransTypeID { get; set; }
        public string TransTypeDesc { get; set; } = string.Empty;
        public string TransTypeCode { get; set; } = string.Empty;
        public int TransTypeModule { get; set; }
        public string? TransTypeMode { get; set; }
    }
}
