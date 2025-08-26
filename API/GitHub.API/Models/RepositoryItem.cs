using System.Text.Json.Serialization;

namespace GitHub.API.Models
{
    public class RepositoryItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("owner")]
        public Owner Owner { get; set; } = new();
    }
}
