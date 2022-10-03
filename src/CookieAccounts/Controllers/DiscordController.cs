using System.Web;
using CookieAccounts.Models.Db;
using Microsoft.AspNetCore.Mvc;

namespace CookieAccounts.Controllers;

[ApiController]
[Route("discord")]
public class DiscordController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly CookieAccountsContext _dbContext;
    private readonly Discord _discord;

    public DiscordController(CookieAccountsContext dbContext, IConfiguration configuration, Discord discord)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _discord = discord;
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult> ReceiveOAuthRedirect([FromQuery] string code, [FromQuery] string state)
    {
        var installationId = HttpUtility.UrlDecode(state);

        var installation = await _dbContext.Installations.FindAsync(installationId);

        var accessToken = await _discord.ExchangeCode(code);

        var userInfo = await _discord.GetUserInformation(accessToken);

        if (userInfo == null) return BadRequest();

        var user = await _dbContext.Users.FindAsync(userInfo.UserId) ?? _dbContext.Users.Add(new User
        {
            UserId = userInfo.UserId
        }).Entity;

        if (installation != null)
        {
            installation.UserId = user.UserId;
            installation.User = user;
        }
        else
        {
            _dbContext.Installations.Add(new Installation
            {
                InstallationId = installationId,
                UserId = user.UserId,
                User = user
            });
        }

        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}