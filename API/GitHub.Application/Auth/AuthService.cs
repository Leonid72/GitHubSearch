using GitHub.Application.Abstractions;
using GitHub.Application.Dtos;
using GitHub.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service responsible for user authentication and registration.
/// Uses the repository pattern for data access, ASP.NET Identity for password hashing,
/// and a JWT generator to issue authentication tokens.
/// </summary>
namespace GitHub.Application.Auth
{
    public class AuthService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher<User> _hasher;
        private readonly IJwtTokenGenerator _jwt;
        private readonly ILogger<AuthService> _logger;
        public AuthService(IUserRepository users, 
                            IPasswordHasher<User> hasher, 
                            IJwtTokenGenerator jwt,
                            ILogger<AuthService> logger)
        {
            _users = users;
            _hasher = hasher;
            _jwt = jwt;
            _logger = logger;
        }
        public sealed record RegisterResult(bool Success, string? AccessToken);
        public sealed record LoginResult(bool Success, string? AccessToken,User? user);
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        public async Task<RegisterResult> RegisterAsync(UserDto dto, CancellationToken ct)
        {
            var exists = await _users.GetByUserNameAsync(dto.UserName, ct);
            if (exists is not null)
            {
                _logger.LogWarning("Register conflict: username already taken");
                return new(false, null);
            }
                    
            // Create a new user and hash their password
            var user = new User { UserName = dto.UserName };
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            await _users.AddAsync(user, ct);
            // Return a signed JWT token for the new user
            var token = _jwt.Generate(user);
            return new(true, token);
        }

        /// <summary>
        /// Logs an existing user into the system.
        /// </summary>
        public async Task<LoginResult> LoginAsync(UserDto dto, CancellationToken ct)
        {
            var user = await _users.GetByUserNameAsync(dto.UserName, ct);
            if (user == null)
            {
                _logger.LogWarning("Login failed: invalid credentials");
                return new(false, null, null);
            }

            // Verify the provided password against the stored hash
            var vr = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (vr == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Login failed: invalid credentials");
                return new(false, null,null);
            }

            // Return a signed JWT token for the authenticated user
            var token = _jwt.Generate(user);
            return new(true, token,user);
        }
    }
}
