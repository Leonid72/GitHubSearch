using System.Text.Json.Serialization;

namespace GitHub.API.Models
{
    public class Owner
    {
        [JsonPropertyName("login")]
        public string Login { get; set; } = string.Empty;
        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; } = string.Empty;
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;
    }
}
