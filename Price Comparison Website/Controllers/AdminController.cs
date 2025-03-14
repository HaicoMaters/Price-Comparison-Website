using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;
using Price_Comparison_Website.Services;

namespace Price_Comparison_Website.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly NotificationService notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Repository<Product> products;
        private readonly Repository<PriceListing> priceListings;
        private readonly Repository<Vendor> vendors;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager){
            _userManager = userManager;
            products = new Repository<Product>(context);
            priceListings = new Repository<PriceListing>(context);
            vendors = new Repository<Vendor>(context);
            notificationService = new NotificationService(context);
        }

        public async Task<IActionResult> Dashboard()
        {
            var prods = await products.GetAllAsync();
            ViewBag.TotalProducts = prods.Count();

            var users = await _userManager.Users.ToListAsync();
            ViewBag.TotalUsers = users.Count();

            var listings = await priceListings.GetAllAsync();
            ViewBag.TotalListings = listings.Count();

            var vends = await vendors.GetAllAsync();
            ViewBag.TotalVendors = vends.Count();

            return View();
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
                await notificationService.CreateGlobalNotification(message);
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