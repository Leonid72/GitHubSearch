using GitHub.API.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GitHub.Infrastructure.Servicies
{
    public interface IGitHubSearchService
    {
        Task<List<GitHubSearchResultDto>> SearchAsync(string query, int page, int perPage);
    }
    /// <summary>
    /// Contract for searching repositories on GitHub using the public REST API.
    /// </summary>
    public class GitHubSearchService: IGitHubSearchService
    {
        private readonly HttpClient _httpClient;

        public GitHubSearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.github.com/");
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DotNetApp", "1.0"));
        }

        public async Task<List<GitHubSearchResultDto>> SearchAsync(string query, int page = 1, int perPage = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query must not be empty.", nameof(query));

            // Escape special characters/spaces in query  
            var encodedQuery = Uri.EscapeDataString($"{query} in:name");
            var url = $"search/repositories?q={encodedQuery}&page={page}&per_page={Math.Clamp(perPage, 1, 100)}&sort=stars&order=desc";
            var response = await _httpClient.GetAsync(url);

            // Throw detailed error if request failed  
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"GitHub API request failed: {response.StatusCode}, {error}");
            }
            response.EnsureSuccessStatusCode();

            // Deserialize JSON into strongly typed model  
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GitHubSearchResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Map GitHubSearchResult to GitHubSearchResultDto  
            var dtoResult = result.Items.Select(item => new GitHubSearchResultDto
            {
                Name = item.Name,
                AvatarUrl = item.Owner.AvatarUrl
            }).ToList();

           return dtoResult;
        }
    }
}
