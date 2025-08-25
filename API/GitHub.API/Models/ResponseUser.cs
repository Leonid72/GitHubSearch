namespace GitHub.API.Models
{
    public class ResponseUser
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Token { get; set; }
    }
}
