
using System.ComponentModel.DataAnnotations;

namespace GitHub.Application.Dtos
{
    public class UserDto
    {
        [Required, MinLength(3)]
        public string UserName { get; set; } = string.Empty;

        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
