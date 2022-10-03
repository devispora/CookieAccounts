using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookieAccounts.Models.Db;

[Table("installations")]
public class Installation
{
    [Key] [Column("installation_id")] public string InstallationId { get; set; }

    [Column("user_id")]
    [ForeignKey("User")]
    public string UserId { get; set; }

    public User? User { get; set; }
}