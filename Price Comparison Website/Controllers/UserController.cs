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

namespace Price_Comparison_Website.Controllers
{
    [Authorize(Roles = "Admin,User")] // Not Currently Needed For Admin Roles But Can Be Changed If Needed Later
    public class UserController : Controller
    {
        private Repository<Product> products;
        private Repository<UserViewingHistory> userViewingHistory;
        private Repository<UserWishList> userWishlists;
        private UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment _webHostEnvironment;
        private NotificationService notificationService;

        public UserController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            products = new Repository<Product>(context);
            userViewingHistory = new Repository<UserViewingHistory>(context);
            userWishlists = new Repository<UserWishList>(context);
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            notificationService = new NotificationService(context);
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
                    var wishlist = await userWishlists.GetAllByIdAsync(user.Id, "UserId", new QueryOptions<UserWishList>());
                    List<Product> productList = new List<Product>();

                    // Add the associated products
                    foreach (var wishlistItem in wishlist)
                    {
                        var product = await products.GetByIdAsync(wishlistItem.ProductId, new QueryOptions<Product>());
                        if (product != null)
                        {
                            productList.Add(product);
                        }
                    }

                    return View(productList);
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { error = "Failed to load wishlist", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
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
                    // Get all viewing histories
                    var viewingHistories = await userViewingHistory.GetAllByIdAsync(user.Id, "UserId", 
                        new QueryOptions<UserViewingHistory>());
                    List<Product> productList = new List<Product>();
                    
                    // Order by most recents first
                    viewingHistories = viewingHistories.OrderByDescending(e => e.LastViewed);

                    // Add the associated products to the viewbag
                    foreach (var history in viewingHistories)
                    {
                        var product = await products.GetByIdAsync(history.ProductId, new QueryOptions<Product>());
                        if (product != null)
                        {
                            productList.Add(product);
                        }
                    }

                    ViewBag.Products = productList;
                    return View(viewingHistories);
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { error = "Failed to load viewing history", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
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
                    var existingEntity = await userWishlists.GetByIdAsync(user.Id, prodId, new QueryOptions<UserWishList>());
                    if (existingEntity == null)
                        return NotFound(new { error = "Wishlist item not found" });

                    await userWishlists.DeleteAsync(existingEntity);
                    return RedirectToAction("Wishlist");
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { error = "Failed to remove from wishlist", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
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
                    // Get all viewing histories
                    var viewingHistories = await userViewingHistory.GetAllByIdAsync(user.Id, "UserId", 
                        new QueryOptions<UserViewingHistory>());

                    foreach (var history in viewingHistories)
                    {
                        await userViewingHistory.DeleteAsync(history);
                    }

                    return RedirectToAction("ViewingHistory");
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { error = "Failed to delete viewing history", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
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
                    var readNotifications = await notificationService.GetReadUserNotifications(user.Id);
                    var unreadNotifications = await notificationService.GetUnreadUserNotifications(user.Id);
                    
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
                    return BadRequest(new { error = "Failed to fetch notifications", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
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
                    await notificationService.MarkNotificationsAsRead(user.Id);
                    return Ok(new { success = true });
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { error = "Failed to mark notifications as read", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
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
                    await notificationService.DeleteUserNotification(notificationId, user.Id);
                    return Ok(new { success = true });
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { error = "Failed to dismiss notification", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }
        
    } 
}

