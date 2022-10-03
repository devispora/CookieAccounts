using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CookieAccounts.Models.Api.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CookieAccounts.Controllers;

[ApiController]
[Route("admin")]
public class AdminController : ControllerBase
{
    private readonly IConfiguration _configuration;


    public AdminController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    public ActionResult<string> GetAdminToken([FromBody] AdminOptions adminOptions)
    {
        var key = Encoding.UTF8.GetBytes
            (_configuration["Jwt:Secret"] ?? string.Empty);

        if (adminOptions.Password != _configuration["Admin:Password"]) return Unauthorized();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Type", "Admin"),
                new Claim("AdminId", adminOptions.Admin),
                new Claim(JwtRegisteredClaimNames.Sub, adminOptions.Admin)
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

        return Ok(jwtToken);
    }
}