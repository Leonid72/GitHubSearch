using GitHub.Application.Auth;
using GitHub.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

/// <summary>
/// Configuration options for JWT authentication.
/// Bound from configuration (e.g. appsettings.json -> "Jwt" section).
/// </summary>
namespace GitHub.Infrastructure.Auth
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";
        public string Issuer { get; init; } = default!;
        public string Audience { get; init; } = default!;
        public string Key { get; init; } = default!;
        public int ExpMinutes { get; init; } = 60;
    }

    /// <summary>
    /// Implementation of JWT token generator using symmetric security keys.
    /// </summary>
    public class JwtTokenGenerator(IOptions<JwtOptions> options) : IJwtTokenGenerator
    {
        private readonly JwtOptions _opt = options.Value;
        public string Generate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            // Create signing credentials using the secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Build the JWT token
            var token = new JwtSecurityToken(
               issuer: _opt.Issuer,
               audience: _opt.Audience,
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(_opt.ExpMinutes),
               signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


}
