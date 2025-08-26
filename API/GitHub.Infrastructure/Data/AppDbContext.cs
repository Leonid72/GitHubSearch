using GitHub.Domain.Users;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Application database context.
/// Inherits from <see cref="DbContext"/> and defines the database sets (tables).
/// </summary>
namespace GitHub.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
    }
}
