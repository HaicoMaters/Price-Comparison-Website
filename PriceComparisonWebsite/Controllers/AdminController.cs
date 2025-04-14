using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IHttpClientFactory _httpClientFactory;
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
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _scraperStatusService = scraperStatusService;
        }

        public async Task<IActionResult> Dashboard()
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

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAllListings()
        {
            // Retrieve the base URL from appsettings.json
            var apiBaseUrl = _configuration["LocalhostUrl"]; // This gets the URL from appsettings.json

            if (string.IsNullOrEmpty(apiBaseUrl))
            {
                _logger.LogError("API Base URL is not configured in appsettings.");
                ViewBag.Message = "Error: API Base URL not configured.";
                return View();
            }

            // Get the authentication cookies from the current request
            var cookies = _httpContextAccessor.HttpContext.Request.Cookies;

            // Add the authentication cookies to the HttpClient
            var cookieContainer = new CookieContainer();
            foreach (var cookie in cookies)
            {
                cookieContainer.Add(new Uri(apiBaseUrl), new Cookie(cookie.Key, cookie.Value));
            }

            var handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer
            };

            // Set the handler on the HttpClient to ensure cookies are sent
            var authenticatedClient = new HttpClient(handler);

            // Call the specific endpoint for updating all listings (PATCH request)
            var requestUri = $"{apiBaseUrl}/api/ScraperApi/update-all-listings";  // Target the correct API endpoint

            try
            {
                // Use HttpRequestMessage to send a PATCH request
                var request = new HttpRequestMessage(HttpMethod.Patch, requestUri);

                var response = await authenticatedClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Listings updated successfully!";
                }
                else
                {
                    ViewBag.Message = $"Failed to update listings. Status: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling ScraperApi");
                ViewBag.Message = $"Error: {ex.Message}";
            }

            return RedirectToAction("Dashboard");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendGlobalNotification(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                ModelState.AddModelError("", "Message cannot be empty");
                return RedirectToAction("Dashboard");
            }

            try
            {
                await _notificationService.CreateGlobalNotification(message);
                TempData["SuccessMessage"] = "Global notification sent successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error sending notification: {ex.Message}";
            }

            return RedirectToAction("Dashboard");
        }
    }
}