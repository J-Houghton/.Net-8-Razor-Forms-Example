using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FormApp.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    } 
    public string ErrorMessage { get; set; }
    public int? StatusCode { get; set; }

    public void OnGet(int? statusCode)
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        StatusCode = statusCode;

        // Custom error messages based on status code
        ErrorMessage = statusCode switch
        {
            404 => "The page you are looking for does not exist.",
            403 => "You are not authorized to access this page.",
            500 => "An internal server error occurred.",
            _ => "An unexpected error occurred."
        };

        // Log the error
        _logger.LogWarning($"Error {statusCode} occurred");
    }
}

