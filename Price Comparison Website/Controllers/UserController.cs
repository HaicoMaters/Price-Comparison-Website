using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Packaging.Signing;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;
using Price_Comparison_Website.Services;
using Price_Comparison_Website.Services.Interfaces;

namespace Price_Comparison_Website.Controllers
{
    [Authorize(Roles = "Admin,User")] 
    public class UserController : Controller
    {
        public readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotificationService _notificationService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IWebHostEnvironment webHostEnvironment, 
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            ILogger<UserController> logger,
            IUserService userService)
        {
            _userService = userService;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _notificationService = notificationService;
            _logger = logger;
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
                    _userService.DeleteViewingHistory(user.Id);
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
            try 
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                try
                {
                    // This is a pretty bad solution but it works for now
                    // Get both read and unread notifications
                    var readNotifications = await _notificationService.GetReadUserNotifications(user.Id);
                    var unreadNotifications = await _notificationService.GetUnreadUserNotifications(user.Id);
                    
                    // Merge both lists and remove duplicates
                    var allNotifications = readNotifications.Concat(unreadNotifications)
                        .GroupBy(n => n.Id) // Remove duplicate notifications by ID
                        .Select(g => g.First()) // Take the first occurrence
                        .OrderByDescending(n => n.CreatedAt) // Sort by newest first
                        .Select(n => new
                        {
                            id = n.Id,
                            message = n.Message,
                            timestamp = n.CreatedAt.ToString("g"),
                            isRead = !unreadNotifications.Any(un => un.Id == n.Id) // Mark as read if not in unread list
                        })
                        .ToList();

                    return Json(new { notifications = allNotifications });
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Failed to fetch notifications for user {UserId}", User.Identity.Name);
                    return BadRequest(new { error = "Failed to fetch notifications", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching notifications for user {UserId}", User.Identity.Name);
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkNotificationsAsRead()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                try
                {
                    await _notificationService.MarkNotificationsAsRead(user.Id);
                    return Ok(new { success = true });
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Failed to mark notifications as read for user {UserId}", User.Identity.Name);
                    return BadRequest(new { error = "Failed to mark notifications as read", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while marking notifications as read for user {UserId}", User.Identity.Name);
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/DismissNotification/{notificationId}")]
        public async Task<IActionResult> DismissNotification(int notificationId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                try
                {
                    await _notificationService.DeleteUserNotification(notificationId, user.Id);
                    return Ok(new { success = true });
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Failed to dismiss notification for user {UserId}", User.Identity.Name);
                    return BadRequest(new { error = "Failed to dismiss notification", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while dismissing notification for user {UserId}", User.Identity.Name);
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }
        
    } 
}

