using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PriceComparisonWebsite.Data;
using PriceComparisonWebsite.Models;
using PriceComparisonWebsite.Services;
using PriceComparisonWebsite.Services.HttpClients;
using PriceComparisonWebsite.Services.WebScraping.Interfaces;

namespace PriceComparisonWebsite.Controllers
{
    /// <summary>
    /// Controller for handling administrative operations
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;
        private readonly ILoginActivityService _loginActivityService;
        private readonly IAdminService _adminService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IScraperStatusService _scraperStatusService;
        private readonly IApiHttpClient _apiClient;

        public AdminController(
            IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            ILogger<AdminController> logger,
            ILoginActivityService loginActivityService,
            IApiHttpClient apiClient,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IScraperStatusService scraperStatusService)
        {
            _userManager = userManager;
            _adminService = adminService;
            _notificationService = notificationService;
            _logger = logger;
            _loginActivityService = loginActivityService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _scraperStatusService = scraperStatusService;
            _apiClient = apiClient;
        }

        /// <summary>
        /// Displays the admin dashboard with various statistics and management options
        /// </summary>
        /// <param name="tab">Optional tab name to display specific section of the dashboard</param>
        /// <returns>The dashboard view with relevant data</returns>
        public async Task<IActionResult> Dashboard(string tab = "notifications")
        {
            var prods = await _adminService.GetAllProductsAsync();
            ViewBag.TotalProducts = prods.Count();

            ViewBag.TotalUsers = await _adminService.GetTotalUsersAsync();

            var listings = await _adminService.GetAllPriceListingsAsync();
            ViewBag.TotalListings = listings.Count();

            var vends = await _adminService.GetAllVendorsAsync();
            ViewBag.TotalVendors = vends.Count();
            ViewBag.VendorsSupportingAutomaticUpdates = vends.Where(v => v.SupportsAutomaticUpdates == true);

            // Get 50 most recent login activities
            var recentActivities = await _loginActivityService.GetNMostRecentActivities(50);
            ViewBag.RecentLoginActivities = recentActivities;

            // Get last automatic update time
            var lastUpdate = await _scraperStatusService.GetLastUpdateTime();
            ViewBag.LastUpdateTime = lastUpdate;

            ViewBag.ActiveTab = tab;

            return View();
        }

        /// <summary>
        /// Triggers an update of all product listings
        /// </summary>
        /// <returns>JSON result indicating success or failure of the update operation</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAllListings()
        {
            try
            {
                var response = await _apiClient.SendAsync(HttpMethod.Patch, "api/ScraperApi/update-all-listings");
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Listings updated sucessfully!" });
                }
                return Json(new { success = false, message = $"Failed to update listings. Status: {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling ScraperApi");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Sends a global notification to all users
        /// </summary>
        /// <param name="message">The notification message to send</param>
        /// <returns>JSON result indicating success or failure of the notification sending</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendGlobalNotification([FromBody] string message)
        {
            try
            {
                var response = await _apiClient.SendAsync(HttpMethod.Post, "api/NotificationApi/create-global-notification", message);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Global notification sent successfully!" });
                }
                return Json(new { success = false, message = $"Failed to send notification. Status: {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling NotificationApi");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}