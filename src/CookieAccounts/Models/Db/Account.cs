using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookieAccounts.Models.Db;

[Table("accounts")]
public class Account
{
    [Key] [Column("account_id")] public string AccountID { get; set; }

    [Required] [Column("token")] public string Token { get; set; }

    public virtual ICollection<EventAccount> EventAccounts { get; set; }
}