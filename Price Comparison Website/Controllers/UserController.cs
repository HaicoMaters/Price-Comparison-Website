﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Controllers
{
    [Authorize(Roles = "User")] // Not Currently Needed For Admin Roles But Can Be Changed If Needed Later
    public class UserController : Controller
    {
        private Repository<Product> products;
        private Repository<UserViewingHistory> userViewingHistory;
        private UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment _webHostEnvironment;

        public UserController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            products = new Repository<Product>(context);
            userViewingHistory = new Repository<UserViewingHistory>(context);
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Wishlist()
        {
            return View();
        }
        public async Task<IActionResult> ViewingHistoryAsync()
        {
            // Get all viewing histories
            var user = await _userManager.GetUserAsync(User);
            var viewingHistories = await userViewingHistory.GetAllByIdAsync(user.Id, "UserId", new QueryOptions<UserViewingHistory>());
            List<Product> productList = new List<Product>();

            // Add the associated products to the viewbag
            foreach (var history in viewingHistories)
            {
                productList.Add(await products.GetByIdAsync(history.ProductId, new QueryOptions<Product>()));
            }

            ViewBag.Products = productList;

            return View(viewingHistories);

    }
    } 
}

