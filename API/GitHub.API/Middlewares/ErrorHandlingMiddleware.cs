
using Microsoft.AspNetCore.Mvc;

namespace GitHub.API.Middlewares
{
    /// <summary>
    /// Middleware that provides centralized error handling for the application.
    /// Converts unhandled exceptions into standardized HTTP responses using <see cref="ProblemDetails"/>.
    /// </summary>
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }
        /// <summary>
        /// Invoked for each HTTP request. Wraps the request pipeline in a try/catch to handle unhandled exceptions.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (System.Exception ex)
            {
                var (status, title) = ex switch
                {
                    ArgumentNullException => (StatusCodes.Status400BadRequest, "A required argument was null."),
                    InvalidOperationException => (StatusCodes.Status409Conflict, "Operation could not be completed."),
                    UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized."),
                    _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
                };

                _logger.LogError(ex, "Unhandled exception ({Status}): {Message}", status, ex.Message);

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = status;
                    context.Response.ContentType = "application/problem+json";

                    var problem = new ProblemDetails
                    {
                        Title = title,
                        Status = status,
                    };

                    await context.Response.WriteAsJsonAsync(problem);
                }
            }
        }
    }
}
