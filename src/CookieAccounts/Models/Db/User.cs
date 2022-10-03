using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookieAccounts.Models.Db;

[Table("users")]
public sealed class User
{
    [Key] [Column("user_id")] public string UserId { get; set; }
    [Column("enabled")] public bool Enabled { get; set; } = true;

    public ICollection<Installation> Installations { get; set; }
    public ICollection<Event> Events { get; set; }
    public ICollection<EventAccount> UsedAccounts { get; set; }
}