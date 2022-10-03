using System.Text.Json.Serialization;

namespace CookieAccounts.Models.Api.Requests;

public class AdminOptions
{
    [JsonPropertyName("password")] public string Password { get; set; }
    [JsonPropertyName("admin")] public string Admin { get; set; }
}