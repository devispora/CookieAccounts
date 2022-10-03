using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CookieAccounts.Models.Api.Requests;

public class EditEvent
{
    [JsonPropertyName("name")] [Required] public string Name { get; set; }

    [JsonPropertyName("accounts")]
    [Required]
    public string[] Accounts { get; set; } = Array.Empty<string>();

    [JsonPropertyName("users")] [Required] public string[] Users { get; set; } = Array.Empty<string>();
    [JsonPropertyName("start")] [Required] public DateTime Start { get; set; }
    [JsonPropertyName("end")] [Required] public DateTime End { get; set; }
}