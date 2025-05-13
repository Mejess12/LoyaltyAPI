using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoyaltyAPI.Models;

public class Transaction
{
    [Key]
    [Column("transaction_id")]
    public int TransactionId { get; set; }

    [Column("date_time")]
    public DateTime DateTime { get; set; }

    [Column("description")]
    public required string Description { get; set; }

    [Column("client_id")]
    public int? ClientId { get; set; }
    [Column("referral_id")]
    public int? ReferralId { get; set; }

    [Column("activity_id")]
    public int ActivityId { get; set; }

    [Column("points")]
    public int? Points { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("reference_id")]
    public string? ReferenceId { get; set; }

    [Column("client_name")]
    public string? ClientName { get; set; }

    [Column("br_id")]
    public int? BrId { get; set; }


}