using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CookieAccounts.Models.Db;

[Table("event_accounts")]
[PrimaryKey("EventId", "AccountId")]
public class EventAccount
{
    [Column("event_id")] public Guid EventId { get; set; }
    [Column("account_id")] public string AccountId { get; set; }
    [Column("used")] public bool Used { get; set; } = false;

    public Event Event { get; set; }
    public Account Account { get; set; }

    [Column("user_id")]
    [ForeignKey("User")]
    public string? UserId { get; set; }

    public User? User { get; set; }
}