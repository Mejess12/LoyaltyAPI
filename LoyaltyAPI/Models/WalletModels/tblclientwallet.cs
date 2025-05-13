namespace LoyaltyAPI.Models
{
    public class ClientWallet
    {

        public ulong Id { get; set; }

        public required string REF_NO { get; set; }
        public int BR_ID { get; set; }
        public int ClientID { get; set; }
        public required string FirstName { get; set; }
        public required string MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public bool IsActive { get; set; }
        public string? ContactNo { get; set; }
    }
}
