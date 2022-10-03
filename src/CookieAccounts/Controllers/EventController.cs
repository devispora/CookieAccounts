using CookieAccounts.Models.Api.Requests;
using CookieAccounts.Models.Api.Responses;
using CookieAccounts.Models.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Event = CookieAccounts.Models.Api.Event;

namespace CookieAccounts.Controllers;

[ApiController]
[Route("events")]
public class EventController : ControllerBase
{
    private readonly CookieAccountsContext _dbContext;
    private readonly ILogger<EventController> _logger;

    public EventController(ILogger<EventController> logger, CookieAccountsContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<GetEventsResponse>> GetEvents()
    {
        var now = DateTime.UtcNow;

        var events = await _dbContext.Events
            .Where(dbEvent => dbEvent.Start - now < TimeSpan.FromMinutes(45) && now < dbEvent.End)
            .Include(dbEvent => dbEvent.Accounts)
            .Include(dbEvent => dbEvent.Users)
            .ToArrayAsync();

        return Ok(new GetEventsResponse
        {
            Events = events.Select(dbEvent =>
                new Event
                {
                    EventId = dbEvent.EventId,
                    Name = dbEvent.Name,
                    Start = dbEvent.Start,
                    End = dbEvent.End,
                    ApplicationId = dbEvent.ApplicationId,
                    Accounts = dbEvent.Accounts.Select(account => account.AccountId).ToArray(),
                    Users = dbEvent.Users.Select(user => user.UserId).ToArray()
                }).ToList()
        });
    }

    [HttpGet("{eventId}")]
    public async Task<ActionResult<Event>> GetEvent(Guid eventId)
    {
        var dbEvent = await _dbContext.Events.Include(dbEvent => dbEvent.Accounts).Include(dbEvent => dbEvent.Users)
            .Where(dbEvent => dbEvent.EventId.Equals(eventId)).FirstOrDefaultAsync();

        if (dbEvent == null) return NotFound();

        return Ok(new Event
            {
                EventId = dbEvent.EventId,
                Name = dbEvent.Name,
                Start = dbEvent.Start,
                End = dbEvent.End,
                ApplicationId = dbEvent.ApplicationId,
                Accounts = dbEvent.Accounts.Select(account => account.AccountId).ToArray(),
                Users = dbEvent.Users.Select(user => user.UserId).ToArray()
            }
        );
    }

    [HttpPost]
    [Authorize(Policy = "Application")]
    public async Task<ActionResult<Event>> CreateEvent([FromBody] CreateEvent eventOptions)
    {
        var applicationId = Guid.Parse(User.Claims.First(claim => claim.Type == "ApplicationId").Value);

        var application = await _dbContext.Applications.FindAsync(applicationId);

        if (application is not { Enabled: true }) return Unauthorized();

        var accounts = await _dbContext.Accounts.Where(account => eventOptions.Accounts.Contains(account.AccountID))
            .ToListAsync();

        var users = await _dbContext.Users.Where(user => eventOptions.Users.Contains(user.UserId))
            .ToListAsync();

        var newEvent = _dbContext.Events.Add(new Models.Db.Event
        {
            Name = eventOptions.Name,
            ApplicationId = applicationId,
            Application = application,
            Start = eventOptions.Start,
            End = eventOptions.End,
            Users = users,
            Accounts = accounts.Select(a => new EventAccount
            {
                AccountId = a.AccountID
            }).ToArray()
        });

        await _dbContext.SaveChangesAsync();

        return Ok(new Event
            {
                EventId = newEvent.Entity.EventId,
                Name = newEvent.Entity.Name,
                Start = newEvent.Entity.Start,
                End = newEvent.Entity.End,
                ApplicationId = newEvent.Entity.ApplicationId,
                Accounts = newEvent.Entity.Accounts.Select(account => account.AccountId).ToArray(),
                Users = newEvent.Entity.Users.Select(user => user.UserId).ToArray()
            }
        );
    }

    [HttpPut("{eventId}")]
    [Authorize(Policy = "Application")]
    public async Task<IActionResult> EditEvent(Guid eventId, [FromBody] EditEvent eventOptions)
    {
        var applicationId = Guid.Parse(User.Claims.First(claim => claim.Type == "ApplicationId").Value);

        var application = await _dbContext.Applications.FindAsync(applicationId);

        if (application is not { Enabled: true }) return Unauthorized();

        var dbEvent = await _dbContext.Events.Include(dbEvent => dbEvent.Accounts).Include(dbEvent => dbEvent.Users)
            .FirstOrDefaultAsync();

        if (dbEvent == null) return NotFound();

        if (!dbEvent.ApplicationId.Equals(applicationId)) return Unauthorized();

        var accounts = await _dbContext.Accounts.Where(account => eventOptions.Accounts.Contains(account.AccountID))
            .ToListAsync();

        var users = await _dbContext.Users.Where(user => eventOptions.Users.Contains(user.UserId))
            .ToListAsync();

        dbEvent.Name = eventOptions.Name;
        dbEvent.Start = eventOptions.Start;
        dbEvent.End = eventOptions.End;
        dbEvent.Users = users;

        foreach (var account in accounts)
        {
            var existingEventAccount =
                dbEvent.Accounts.FirstOrDefault(eventAccount => eventAccount.AccountId.Equals(account.AccountID));

            if (existingEventAccount == null)
                dbEvent.Accounts.Add(new EventAccount
                {
                    AccountId = account.AccountID,
                    EventId = dbEvent.EventId
                });
        }

        foreach (var eventAccount in dbEvent.Accounts)
            if (accounts.Find(account => account.AccountID == eventAccount.AccountId) == null)
                dbEvent.Accounts.Remove(eventAccount);

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}