using GitHub.Domain.Users;

namespace GitHub.Application.Auth
{
    public interface IJwtTokenGenerator
    {
        string Generate(User user);
    }
}
