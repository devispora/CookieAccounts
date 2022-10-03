using System.Text.Json.Serialization;

namespace CookieAccounts.Models.Api.Responses;

public class CreateApplicationResponse
{
    [JsonPropertyName("application")] public Application Application { get; set; }

    [JsonPropertyName("token")] public string Token { get; set; }
}