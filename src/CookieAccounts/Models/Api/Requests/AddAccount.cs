using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CookieAccounts.Models.Api.Requests;

public class AddAccount
{
    [JsonPropertyName("accountId")]
    [Required]
    public string AccountId { get; set; }

    [JsonPropertyName("token")] [Required] public string Token { get; set; }
}