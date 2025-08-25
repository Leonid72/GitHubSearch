using GitHub.Domain.Users;

/// <summary>
/// Defines a contract for accessing and manipulating User entities.
/// This follows the Repository Pattern, which abstracts away
/// the details of data access (EF Core, SQL, etc.).
/// </summary>
namespace GitHub.Application.Abstractions
{
    public interface IUserRepository
    {
        Task<User?> GetByUserNameAsync(string userName, CancellationToken ct);
        Task<User?> GetByUserIdAsync(int id, CancellationToken ct); 
        Task AddAsync(User user, CancellationToken ct);
        Task UpdateAsync(User user, CancellationToken ct);
    }
}
