using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using CookieAccounts.Models.Api.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CookieAccounts.Controllers;

[ApiController]
[Route("installations")]
public class InstallationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly CookieAccountsContext _dbContext;

    public InstallationController(IConfiguration configuration, CookieAccountsContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    [HttpGet("register")]
    public IActionResult Register([FromQuery] string installationId)
    {
        var clientId = _configuration["Discord:ClientId"];
        var scopes = HttpUtility.UrlEncode("identify");
        var state = HttpUtility.HtmlEncode(installationId);
        var redirectUri = _configuration["Discord:RedirectUri"];

        var oauthUrl =
            $"https://discord.com/oauth2/authorize?response_type=code&client_id={clientId}&scope={scopes}&state={state}&redirect_uri={redirectUri}&prompt=none";

        return Redirect(oauthUrl);
    }

    [HttpGet("{installationId}")]
    public async Task<ActionResult<GetAuthenticationResponse>> GetAuthentication(string installationId)
    {
        var installation = await _dbContext.Installations.FindAsync(installationId);

        if (installation == null) return Unauthorized();

        var key = Encoding.UTF8.GetBytes
            (_configuration["Jwt:Secret"] ?? string.Empty);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Type", "User"),
                new Claim("UserId", installation.UserId),
                new Claim(JwtRegisteredClaimNames.Sub, installation.UserId)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return Ok(new GetAuthenticationResponse
        {
            Token = jwtToken
        });
    }
}