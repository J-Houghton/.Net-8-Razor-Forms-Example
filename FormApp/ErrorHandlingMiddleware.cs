namespace FormApp
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Check for unhandled status codes
                if (context.Response.StatusCode >= 400)
                {
                    context.Response.Redirect($"/Error?statusCode={context.Response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");

                context.Response.Redirect("/Error?statusCode=500");
            }
        }
    }

    // Extension method
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
} 