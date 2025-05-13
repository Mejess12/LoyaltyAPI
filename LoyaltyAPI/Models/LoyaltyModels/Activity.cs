using System;
using System.ComponentModel.DataAnnotations;


public class Activity
{
    [Key]
    public int activity_id { get; set; }
    public required string description { get; set; }
    public int points { get; set; }
    public DateTime date_from { get; set; }
    public DateTime date_to { get; set; }
    public DateTime date_time { get; set; }
    public int system_source_id { get; set; }
    public DateTime created_at { get; set; }
    public string? transaction_type { get; set; }
}