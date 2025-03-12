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

        [Authorize (Roles = "User")]
        public async Task<IActionResult> Wishlist()
        {
            // Get all wishlist
            var user = await _userManager.GetUserAsync(User);
            var wishlist = await userWishlists.GetAllByIdAsync(user.Id, "UserId", new QueryOptions<UserWishList>());
            List<Product> productList = new List<Product>();

            // Add the associated products
            foreach (var wishlistItem in wishlist)
            {
                productList.Add(await products.GetByIdAsync(wishlistItem.ProductId, new QueryOptions<Product>()));
            }

            return View(productList);
        }

        [Authorize (Roles = "User")]
        public async Task<IActionResult> ViewingHistory()
        {
            // Get all viewing histories
            var user = await _userManager.GetUserAsync(User);
            var viewingHistories = await userViewingHistory.GetAllByIdAsync(user.Id, "UserId", new QueryOptions<UserViewingHistory> ());
            List<Product> productList = new List<Product>();
            viewingHistories = viewingHistories.OrderByDescending(e => e.LastViewed); // Order by most recents first

            // Add the associated products to the viewbag
            foreach (var history in viewingHistories)
            {
                productList.Add(await products.GetByIdAsync(history.ProductId, new QueryOptions<Product>()));
            }

            ViewBag.Products = productList;

            return View(viewingHistories);
        }
    
        [Authorize (Roles = "User")]
        public async Task<IActionResult> RemoveFromWishlist(int prodId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var existingEntity = await userWishlists.GetByIdAsync(user.Id, prodId, new QueryOptions<UserWishList>());

                // Delete from wishlist
                if (existingEntity != null)
                {
                    try
                    {
                        await userWishlists.DeleteAsync(existingEntity);
                    }
                    catch (InvalidOperationException ex)
                    {
                        ModelState.AddModelError("", ex.Message); // Handle deletion issue
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error deleting product: {ex.GetBaseException().Message}"); // General error handler
                    }
                }
            }

            return RedirectToAction("Wishlist");
        }

        [Authorize (Roles = "User")]
        public async Task<IActionResult> DeleteViewingHistory()
        {
            // Get all viewing histories
            var user = await _userManager.GetUserAsync(User);
            var viewingHistories = await userViewingHistory.GetAllByIdAsync(user.Id, "UserId", new QueryOptions<UserViewingHistory>());

            foreach (UserViewingHistory history in viewingHistories)
            {
                try
                {
                    await userViewingHistory.DeleteAsync(history);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error deleting product: {ex.GetBaseException().Message}");
                }
            }

                return RedirectToAction("ViewingHistory");
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            try 
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

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
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while fetching notifications" });
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

                await notificationService.MarkNotificationsAsRead(user.Id);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while marking notifications as read" });
            }
        }
    } 
}

