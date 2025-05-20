using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Controllers
{
    /// <summary>
    /// Controller for handling different types of errors and status codes
    /// </summary>
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handles HTTP status code errors and returns appropriate error pages
        /// </summary>
        /// <param name="statusCode">The HTTP status code that triggered the error</param>
        /// <returns>The error view with appropriate error messages</returns>
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            _logger.LogWarning("HTTP {StatusCode} error occurred. Path: {Path}",
                statusCode, statusCodeResult?.OriginalPath);

            switch (statusCode)
            {
                case 404:
                    ViewData["ErrorCode"] = "404 - Page Not Found";
                    ViewData["ErrorMessage"] = "Sorry, the page you requested could not be found.";
                    break;
                case 401:
                    ViewData["ErrorCode"] = "401 - Unauthorized";
                    ViewData["ErrorMessage"] = "You are not authorized to access this resource.";
                    break;
                case 403:
                    ViewData["ErrorCode"] = "403 - Forbidden";
                    ViewData["ErrorMessage"] = "You don't have permission to access this resource.";
                    break;
                default:
                    ViewData["ErrorCode"] = statusCode + " - Error";
                    ViewData["ErrorMessage"] = "An error occurred while processing your request.";
                    break;
            }

            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Handles unhandled exceptions and returns a generic error page
        /// </summary>
        /// <returns>The error view with generic error message</returns>
        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            _logger.LogError(exceptionDetails?.Error,
                "An unhandled exception occurred. Path: {Path}",
                exceptionDetails?.Path);

            ViewData["ErrorCode"] = "Error";
            ViewData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
