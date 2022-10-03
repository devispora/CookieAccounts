using System.Text.Json.Serialization;

namespace CookieAccounts.Models.Api.Responses;

public class GetAuthenticationResponse
{
    [JsonPropertyName("token")] public string Token { get; set; }
}