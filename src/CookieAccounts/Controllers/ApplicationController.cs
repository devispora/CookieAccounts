using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CookieAccounts.Models.Api.Requests;
using CookieAccounts.Models.Api.Responses;
using CookieAccounts.Models.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CookieAccounts.Controllers;

[ApiController]
[Route("applications")]
[Produces("application/json")]
[Authorize]
public class ApplicationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly CookieAccountsContext _dbContext;
    private readonly ILogger<ApplicationController> _logger;

    public ApplicationController(ILogger<ApplicationController> logger, IConfiguration configuration,
        CookieAccountsContext dbContext)
    {
        _logger = logger;
        _configuration = configuration;
        _dbContext = dbContext;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateApplicationResponse))]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<CreateApplicationResponse>> CreateApplication(
        [FromBody] CreateApplication applicationOptions)
    {
        var application = _dbContext.Applications.Add(new Application
        {
            Name = applicationOptions.Name
        });

        await _dbContext.SaveChangesAsync();

        var key = Encoding.UTF8.GetBytes
            (_configuration["Jwt:Secret"] ?? string.Empty);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Type", "Application"),
                new Claim("ApplicationId", application.Entity.ApplicationId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, application.Entity.ApplicationId.ToString())
            }),
            Expires = DateTime.UtcNow.AddYears(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return Ok(new CreateApplicationResponse
        {
            Application = new Models.Api.Application
            {
                ApplicationId = application.Entity.ApplicationId,
                Name = application.Entity.Name
            },
            Token = jwtToken
        });
    }
}