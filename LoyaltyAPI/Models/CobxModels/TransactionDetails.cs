using System.ComponentModel.DataAnnotations.Schema;

namespace LoyaltyAPI.Models
{
    public class TransactionDetails
    {
        [Column("TransDetailID")]
        public int TransDetailID { get; set; }
        public long BR_ID { get; set; }
        public short Branch_Code { get; set; }
        public short? TransactionCode { get; set; }
        public int TransYear { get; set; }
        public long CTLNo { get; set; }
        public long AccountCode { get; set; }
        public long ClientID { get; set; }
        public byte SLC_Code { get; set; }
        public byte SLT_Code { get; set; }
        public string ReferenceNo { get; set; } = string.Empty;
        public byte SLE_Code { get; set; }
        public byte StatusID { get; set; }
        public DateTime SLDate { get; set; }
        public byte AdjFlag { get; set; }
        public decimal Amt { get; set; }
        public short EncodedBy { get; set; }
        public short ApprovedBy { get; set; }
        public short PostedBy { get; set; }
        public short UPDTag { get; set; }
        public int SequenceNo { get; set; }
        public string? ClientName { get; set; }

    }
}