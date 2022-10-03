using System.Text.Json.Serialization;

namespace CookieAccounts.Models.Api.Responses;

public class GetEventsResponse
{
    [JsonPropertyName("events")] public List<Event> Events { get; set; } = new();
}