using System.ComponentModel.DataAnnotations;

namespace GitHub.Domain.Users
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required,MaxLength(20)]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
    }
}
