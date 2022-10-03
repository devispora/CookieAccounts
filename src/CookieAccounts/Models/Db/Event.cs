using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookieAccounts.Models.Db;

[Table("events")]
public class Event
{
    [Key] [Column("event_id")] public Guid EventId { get; set; } = new();

    [Required] [Column("name")] public string Name { get; set; }

    [Required] [Column("start")] public DateTime Start { get; set; } = DateTime.UtcNow;

    [Required] [Column("end")] public DateTime End { get; set; } = DateTime.UtcNow.AddHours(1);

    [Required] [Column("application_id")] public Guid ApplicationId { get; set; }

    public Application Application { get; set; }

    public virtual ICollection<EventAccount> Accounts { get; set; }
    public virtual ICollection<User> Users { get; set; }
}