using System.Text.Json.Serialization;

namespace CookieAccounts.Models.Api.Responses;

public class GetUsersResponse
{
    [JsonPropertyName("users")] public User[] Users { get; set; } = Array.Empty<User>();
}