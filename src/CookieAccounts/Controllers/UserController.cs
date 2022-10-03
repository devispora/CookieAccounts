using CookieAccounts.Models.Api;
using CookieAccounts.Models.Api.Requests;
using CookieAccounts.Models.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookieAccounts.Controllers;

[ApiController]
[Route("users")]
[Authorize("Application")]
public class UserController : ControllerBase
{
    private readonly CookieAccountsContext _dbContext;
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger, CookieAccountsContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUser userOptions)
    {
        var existingUser = await _dbContext.Users.Where(user => user.UserId.Equals(userOptions.DiscordId))
            .FirstOrDefaultAsync();

        if (existingUser != null) return BadRequest();

        var user = _dbContext.Users.Add(new Models.Db.User
        {
            UserId = userOptions.DiscordId
        });

        await _dbContext.SaveChangesAsync();

        return Ok(new User
        {
            UserId = user.Entity.UserId
        });
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<User>> GetUser(string userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);

        if (user == null) return NotFound();

        return Ok(new User
        {
            UserId = user.UserId
        });
    }

    [HttpGet]
    public async Task<ActionResult<GetUsersResponse>> GetUsers([FromQuery] string users)
    {
        var userIds = users.Split(",").Where(userId => userId.Length > 0).ToArray();

        var dbUsers = await _dbContext.Users.Where(user => userIds.Contains(user.UserId)).ToArrayAsync();

        return Ok(new GetUsersResponse
        {
            Users = dbUsers.Select(user => new User
            {
                UserId = user.UserId
            }).ToArray()
        });
    }
}