using System.Text.Json.Serialization;

namespace CookieAccounts.Models.Api;

public class Event
{
    [JsonPropertyName("eventId")] public Guid EventId { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("start")] public DateTime Start { get; set; }

    [JsonPropertyName("end")] public DateTime End { get; set; }

    [JsonPropertyName("applicationId")] public Guid ApplicationId { get; set; }

    [JsonPropertyName("accounts")] public string[] Accounts { get; set; }

    [JsonPropertyName("users")] public string[] Users { get; set; }
}