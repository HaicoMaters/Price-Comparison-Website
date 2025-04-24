using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Packaging.Signing;
using PriceComparisonWebsite.Data;
using PriceComparisonWebsite.Models;
using PriceComparisonWebsite.Services;
using PriceComparisonWebsite.Services.Interfaces;
using System.Net;
using System.Net.Http;
using PriceComparisonWebsite.Services.HttpClients;

namespace PriceComparisonWebsite.Controllers
{
    [Authorize(Roles = "Admin,User")] 
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IApiHttpClient _apiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            ILogger<UserController> logger,
            IUserService userService,
            IConfiguration configuration,
            IApiHttpClient apiClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _userManager = userManager;
            _notificationService = notificationService;
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _apiClient = apiClient;
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Wishlist()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                try
                {
                    // Get all wishlist
                    var wishlist = await _userService.GetUserWishListItems(user.Id);
                    // Add the associated products
                    var productList = await _userService.GetProductsFromWishList(wishlist);

                    return View(productList);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Failed to load wishlist for user {UserId}", User.Identity.Name);
                    return BadRequest(new { error = "Failed to load wishlist", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while loading wishlist for user {UserId}", User.Identity.Name);
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> ViewingHistory()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                try
                {
                    // Get all viewing histories ordered by most recent
                    var viewingHistories = await _userService.GetUserViewingHistory(user.Id);

                    ViewBag.Products = await _userService.GetProductsFromViewingHistory(viewingHistories);
                    return View(viewingHistories);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Failed to load viewing history for user {UserId}", User.Identity.Name);
                    return BadRequest(new { error = "Failed to load viewing history", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while loading viewing history for user {UserId}", User.Identity.Name);
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromWishlist(int prodId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                try
                {
                    await _userService.RemoveFromWishlist(prodId, user.Id);
                    return RedirectToAction("Wishlist");
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Failed to remove from wishlist for user {UserId}", User.Identity.Name);
                    return BadRequest(new { error = "Failed to remove from wishlist", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while removing from wishlist for user {UserId}", User.Identity.Name);
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteViewingHistory()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                try
                {
                    await _userService.DeleteViewingHistory(user.Id);
                    return RedirectToAction("ViewingHistory");
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Failed to delete viewing history for user {UserId}", User.Identity.Name);
                    return BadRequest(new { error = "Failed to delete viewing history", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting viewing history for user {UserId}", User.Identity.Name);
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            try
            {
                var response = await _apiClient.SendAsync(HttpMethod.Get, $"api/NotificationApi/user-notifications/{user.Id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Content(content, "application/json");
                }
                
                return StatusCode((int)response.StatusCode, new { success = false, message = $"Failed to fetch notifications. Status: {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling NotificationApi");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkNotificationsAsRead()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            try
            {
                var response = await _apiClient.SendAsync(HttpMethod.Post, $"api/NotificationApi/mark-as-read/{user.Id}");

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Notifications marked as read successfully!" });
                }
                
                return Json(new { success = false, message = $"Failed to mark notifications as read. Status: {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling NotificationApi");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/DismissNotification/{notificationId}")]
        public async Task<IActionResult> DismissNotification(int notificationId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            try
            {
                var response = await _apiClient.SendAsync(HttpMethod.Post, $"api/NotificationApi/dismiss/{user.Id}/{notificationId}");

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Notification dismissed successfully!" });
                }
                
                return Json(new { success = false, message = $"Failed to dismiss notification. Status: {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling NotificationApi");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    } 
}

