
namespace GitHub.API.Controllers
{
    /// <summary>
    /// Controller responsible for handling GitHub search requests.
    /// Inherits from BaseController for common API behaviors (like unified error handling, logging, etc.).
    /// </summary>

    [Authorize]
    public class GitHubController : BaseController
    {
        private readonly IGitHubSearchService _gitHubSearchService;
        private readonly IGenericCacheService _cacheService;
        public GitHubController(IGitHubSearchService gitHubSearchService, IGenericCacheService cacheService)
        {
            _gitHubSearchService = gitHubSearchService;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Searches GitHub repositories by keyword.
        /// </summary>
        /// <remarks>
        /// Requires authorization (Bearer JWT).
        /// Example: <c>GET /api/GitHub/search?keyword=angular&amp;page=1&amp;perPage=20</c>.
        /// </remarks>
        [HttpGet("search")]
        public async Task<ActionResult> Search([FromQuery] string keyword, [FromQuery] int page = 1, [FromQuery] int perPage = 20)
        {
            keyword = keyword?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Keyword is required.");
            try
            {
                // Call service to fetch repositories from GitHub API
                var result = await _gitHubSearchService.SearchAsync(keyword, page, perPage);
                return Ok(result);

            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error: {ex.Message}");
            }
        }

        [HttpPost("bookmarks/{userId:int}")]
        public ActionResult AddBookmark([FromRoute] int  userId, [FromBody] GitHubSearchResultDto repo)
        {
            // Validate payload
            if (repo is null || string.IsNullOrWhiteSpace(repo.Name))
                return BadRequest(new ProblemDetails { Title = "Valid repository payload required.", 
                                                       Status = StatusCodes.Status400BadRequest });
            // Retrieve existing bookmarks from cache
            var key = $"bookmarks:{userId}";
            var bookmarks = _cacheService.Get<List<GitHubSearchResultDto>>(key) ?? new List<GitHubSearchResultDto>();
            if (!bookmarks.Any(b => b.Name == repo.Name))
                bookmarks.Add(repo);
            // Update cache
            _cacheService.Set(key, bookmarks);

            return Ok(bookmarks);
        }

        [HttpDelete("bookmarks/{userId:int}/{name}")]
        public ActionResult RemoveBookmarkByName([FromRoute] int userId, [FromRoute] string name)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new ProblemDetails { Title = "Repository name required.", 
                                                       Status = StatusCodes.Status400BadRequest });
            // Retrieve existing bookmarks from cache
            var key = $"bookmarks:{userId}";
            var bookmarks = _cacheService.Get<List<GitHubSearchResultDto>>(key) ?? new();

            // Remove all matches (case-insensitive) to be safe against duplicates
            var removedCount = bookmarks.RemoveAll(b => string.Equals(b.Name, name, StringComparison.OrdinalIgnoreCase));

            if (removedCount == 0)
                return NotFound($"Bookmark '{name}' not found.");

            _cacheService.Set(key, bookmarks);
            return Ok(bookmarks); // updated list
        }

        [HttpGet("bookmarks/{userId:int}")]
        public ActionResult<List<GitHubSearchResultDto>> GetBookmarks([FromRoute] int userId)
        {
            // Retrieve bookmarks from cache
            var key = $"bookmarks:{userId}";
            var bookmarks = _cacheService.Get<List<GitHubSearchResultDto>>(key) ?? new List<GitHubSearchResultDto>();
            return Ok(bookmarks);
        }
    }
}
