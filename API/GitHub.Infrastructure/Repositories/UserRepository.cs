
using GitHub.Application.Abstractions;
using GitHub.Domain.Users;
using GitHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository for working with the User entity (Repository Pattern).
/// </summary>
namespace GitHub.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext db) : IUserRepository
    {
        private readonly AppDbContext _db = db;
        public async Task AddAsync(User user, CancellationToken ct)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(User user, CancellationToken ct)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);
        }

        public Task<User?> GetByUserNameAsync(string userName, CancellationToken ct) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == userName, ct);
        public Task<User?> GetByUserIdAsync(int id, CancellationToken ct) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
    }
}
