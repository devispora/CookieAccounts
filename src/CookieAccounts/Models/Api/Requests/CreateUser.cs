using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CookieAccounts.Models.Api.Requests;

public class CreateUser
{
    [JsonPropertyName("discordId")]
    [Required]
    public string DiscordId { get; set; }
}