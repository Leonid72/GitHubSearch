namespace GitHub.API.Controllers
{
    public class AuthController(AuthService auth, IUserRepository user) : BaseController
    {
        private readonly AuthService _auth = auth;
        private readonly IUserRepository _users = user;
        public record AuthenticatedResponse(string Token);
        /// <summary>
        /// Registers a new user account and returns an access token on success.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ResponseUser>> Register(UserDto dto, CancellationToken ct)
        {
            var result = await _auth.RegisterAsync(dto, ct);
            if (!result.Success)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "UserName already taken",
                    Status = StatusCodes.Status409Conflict
                });
            }
            return new ResponseUser { UserName = dto.UserName, Token = result.AccessToken };
        }

        /// <summary>
        /// Authenticates an existing user and returns an access token.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseUser>> Login(UserDto dto, CancellationToken ct)
        {
            var result = await _auth.LoginAsync(dto, ct);
            if (!result.Success)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid username or password",
                    Status = StatusCodes.Status401Unauthorized
                });
            }
            return new ResponseUser { Id = result.user.Id, UserName = result.user.UserName, Token = result.AccessToken };
        }

        /// <summary>
        /// Returns the authenticated user's info.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(ResponseUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseUser>> Me(CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim is null || string.IsNullOrWhiteSpace(userIdClaim.Value))
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "User not authenticated",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            if (string.IsNullOrWhiteSpace(userIdClaim.Value))
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid user Name in token",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            var user = await _users.GetByUserNameAsync(userIdClaim.Value, ct);
            if (user is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User not found",
                    Status = StatusCodes.Status404NotFound
                });
            }
            return Ok(new ResponseUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = null
            });
        }

    }
}
