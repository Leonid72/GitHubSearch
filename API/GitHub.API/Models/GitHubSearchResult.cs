using System.Text.Json.Serialization;

namespace GitHub.API.Models
{
    public class GitHubSearchResult
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("incomplete_results")]
        public bool IncompleteResults { get; set; }

        [JsonPropertyName("items")]
        public List<RepositoryItem> Items { get; set; } = new();
    }
}
