using System.Text.Json.Serialization;

namespace CookieAccounts.Models.Api;

public class User
{
    [JsonPropertyName("userId")] public string UserId { get; set; }
}