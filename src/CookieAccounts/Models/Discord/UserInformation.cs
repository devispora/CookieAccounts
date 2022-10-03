using System.Text.Json.Serialization;

namespace CookieAccounts.Models.Discord;

public class UserInformation
{
    [JsonPropertyName("id")] public string UserId { get; set; }
}