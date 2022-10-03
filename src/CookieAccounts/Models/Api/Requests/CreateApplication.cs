using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CookieAccounts.Models.Api.Requests;

public class CreateApplication
{
    [JsonPropertyName("name")] [Required] public string Name { get; set; }
}