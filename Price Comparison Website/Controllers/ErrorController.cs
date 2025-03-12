using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

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

        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            
            ViewData["ErrorCode"] = "Error";
            ViewData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
