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
using PriceComparisonWebsite.Services.WebScraping.Interfaces;

namespace PriceComparisonWebsite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;
        private readonly ILoginActivityService _loginActivityService;
        private readonly IAdminService _adminService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IScraperStatusService _scraperStatusService;

        public AdminController(
            IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            ILogger<AdminController> logger,
            ILoginActivityService loginActivityService,
            IHttpClientFactory httpClientFactory,
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
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task<IActionResult> Dashboard(string tab = "notifications")
        {
            var prods = await _adminService.GetAllProductsAsync();
            ViewBag.TotalProducts = prods.Count();

            var users = await _userManager.Users.ToListAsync();
            ViewBag.TotalUsers = users.Count();

            var listings = await _adminService.GetAllPriceListingsAsync();
            ViewBag.TotalListings = listings.Count();

            var vends = await _adminService.GetAllVendorsAsync();
            ViewBag.TotalVendors = vends.Count();
            ViewBag.VendorsSupportingAutomaticUpdates = vends.Where(v => v.SupportsAutomaticUpdates == true);

            // Get 10 most recent login activities
            var recentActivities = await _loginActivityService.GetNMostRecentActivities(50);
            ViewBag.RecentLoginActivities = recentActivities;

            // Get last automatic update time
            var lastUpdate = await _scraperStatusService.GetLastUpdateTime();
            ViewBag.LastUpdateTime = lastUpdate;

            ViewBag.ActiveTab = tab;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAllListings()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Patch, "api/ScraperApi/update-all-listings");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Listings update process started successfully!" });
                }
                return Json(new { success = false, message = $"Failed to update listings. Status: {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling ScraperApi");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken] /// FIX THIS SO MESSAGE SENDS CORRECTLY AND NOT NULL OR EMPTY
        public async Task<IActionResult> SendGlobalNotification(string message)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"api/NotificationApi/create-global-notification/{message}");
                var response = await _httpClient.SendAsync(request);

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