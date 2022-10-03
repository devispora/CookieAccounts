using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CookieAccounts.Models.Api.Requests;

public class CreateEvent
{
    [JsonPropertyName("name")] [Required] public string Name { get; set; }

    [JsonPropertyName("accounts")]
    [Required]
    public string[] Accounts { get; set; }

    [JsonPropertyName("users")] [Required] public string[] Users { get; set; }
    [JsonPropertyName("start")] [Required] public DateTime Start { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("end")] [Required] public DateTime End { get; set; } = DateTime.UtcNow.AddHours(1);
}