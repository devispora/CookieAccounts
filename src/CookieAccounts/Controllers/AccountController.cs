using CookieAccounts.Models.Api.Requests;
using CookieAccounts.Models.Api.Responses;
using CookieAccounts.Models.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookieAccounts.Controllers;

[ApiController]
[Route("accounts")]
[Produces("application/json")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly CookieAccountsContext _dbContext;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger,
        CookieAccountsContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet("")]
    [Authorize("User")]
    public async Task<ActionResult<GetAccountResponse>> GetAccount()
    {
        var userId = User.Claims.First(claim => claim.Type == "UserId").Value;

        var user = await _dbContext.Users.FindAsync(userId);

        if (user is not { Enabled: true }) return Unauthorized();

        var now = DateTime.UtcNow;

        var currentEvent =
            await _dbContext.Events.Where(dbEvent => dbEvent.Users.Contains(user))
                .Where(dbEvent => dbEvent.End > now)
                .Where(dbEvent => dbEvent.Start < now)
                .Include(dbEvent => dbEvent.Accounts)
                .ThenInclude(eventAccount => eventAccount.Account)
                .FirstOrDefaultAsync();

        if (currentEvent == null) return NotFound();

        var existingAccount = currentEvent.Accounts.FirstOrDefault(account => account.UserId == userId);

        if (existingAccount != null)
            return Ok(new GetAccountResponse
            {
                AccountId = existingAccount.AccountId,
                Token = existingAccount.Account.Token
            });

        var availableAccount = currentEvent.Accounts.FirstOrDefault(account => !account.Used);

        if (availableAccount == null) return BadRequest();

        availableAccount.User = user;
        availableAccount.UserId = userId;
        availableAccount.Used = true;

        await _dbContext.SaveChangesAsync();

        return Ok(new GetAccountResponse
        {
            AccountId = availableAccount.AccountId,
            Token = availableAccount.Account.Token
        });
    }

    [HttpPost]
    [Authorize("Admin")]
    public async Task<IActionResult> AddAccount([FromBody] AddAccount accountOptions)
    {
        _dbContext.Accounts.Add(new Account
        {
            AccountID = accountOptions.AccountId,
            Token = accountOptions.Token
        });

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}