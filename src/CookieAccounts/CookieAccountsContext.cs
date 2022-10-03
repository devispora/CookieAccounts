using CookieAccounts.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace CookieAccounts;

public class CookieAccountsContext : DbContext
{
    public CookieAccountsContext(DbContextOptions<CookieAccountsContext> options) : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<Installation> Installations { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}