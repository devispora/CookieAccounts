using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookieAccounts.Models.Db;

[Table("applications")]
public class Application
{
    [Key] [Column("application_id")] public Guid ApplicationId { get; set; } = new();

    [Required] [Column("name")] public string Name { get; set; }

    [Required] [Column("enabled")] public bool Enabled { get; set; } = true;

    public virtual ICollection<Event> Events { get; set; }
}