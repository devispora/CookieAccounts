using System.Text.Json.Serialization;

namespace CookieAccounts.Models.Api;

public class Application
{
    [JsonPropertyName("applicationId")] public Guid ApplicationId { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }
}