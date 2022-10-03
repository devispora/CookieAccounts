using System.Net.Http.Headers;
using CookieAccounts.Models.Discord;

namespace CookieAccounts;

public class Discord
{
    private const string ApiEndpoint = "https://discord.com/api/v10";
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public Discord(IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(ApiEndpoint)
        };
    }

    public async Task<string> ExchangeCode(string code)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "oauth2/token");

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var clientId = _configuration["Discord:ClientId"];
        var clientSecret = _configuration["Discord:ClientSecret"];
        var redirectUri = _configuration["Discord:RedirectUri"];

        var body = new List<KeyValuePair<string, string>>
        {
            new("client_id", clientId ?? throw new InvalidOperationException()),
            new("client_secret", clientSecret ?? throw new InvalidOperationException()),
            new("grant_type", "authorization_code"),
            new("code", code),
            new("redirect_uri", redirectUri ?? throw new InvalidOperationException())
        };

        request.Content = new FormUrlEncodedContent(body);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        var response = await _httpClient.SendAsync(request);

        var responseContent = await response.Content.ReadFromJsonAsync<ExchangeCodeResponse>();

        return responseContent?.AccessToken ?? throw new InvalidOperationException();
    }

    public async Task<UserInformation?> GetUserInformation(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "users/@me");

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        var responseContent = await response.Content.ReadFromJsonAsync<UserInformation>();

        return responseContent;
    }
}