﻿
using System.Drawing.Printing;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Controllers
{
    public class ProductController : Controller
    {
		private Repository<Product> products;
		private Repository<Category> categories;
        private Repository<PriceListing> priceListings;
        private Repository<Vendor> vendors;
        private Repository<UserViewingHistory> userViewingHistory;
        private Repository<UserWishList> userWishlists;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            products = new Repository<Product>(context);
            categories = new Repository<Category>(context);
            priceListings = new Repository<PriceListing>(context);
            vendors = new Repository<Vendor>(context);
            userViewingHistory = new Repository<UserViewingHistory>(context);
            userWishlists = new Repository<UserWishList>(context);
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int catId = 0, string searchQuery = "")
        {
            ViewBag.Categories = await categories.GetAllAsync();

            IEnumerable<Product> allProducts;

            if (catId == 0) // Search All Categories
            {
                allProducts = await products.GetAllAsync();
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    // Filter products based on the search query
                    allProducts = allProducts
                        .Where(p => (p.Name != null && p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                        (p.Description != null && p.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }
            }
            else // Search Products by CategoryId
            {
                allProducts = await products.GetAllByIdAsync(catId, "CategoryId", new QueryOptions<Product>
                {
                    Includes = "Category",
                    Where = p => p.CategoryId == catId
                });

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    // Filter products based on the search query
                    allProducts = allProducts
                        .Where(p => (p.Name != null && p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                        (p.Description != null && p.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }
            }

            // Paginate the products
            var pagedProducts = SetupPagination(allProducts, pageNumber);
            
            // Handle User Specific Data
            if (User.Identity.IsAuthenticated)
            {

                var user = await _userManager.GetUserAsync(User);
                if (user != null && await _userManager.IsInRoleAsync(user, "User"))
                {
                    // Get which items are on the wishlist
                    List<bool> onWishlist = new List<bool>();
                    for (int i = 0; i < pagedProducts.Count; i++)
                    {
                        var existingEntity = await userWishlists.GetByIdAsync(user.Id, pagedProducts[i].ProductId, new QueryOptions<UserWishList>());
                        if(existingEntity != null)
                        {
                            onWishlist.Add(true);
                        }
                        else
                        {
                            onWishlist.Add(false);
                        }
                    }
                    ViewBag.OnWishlist = onWishlist;
                }
            }

            // Calculate total pages and set ViewData
            ViewData["CategoryId"] = catId;
            ViewData["SearchQuery"] = searchQuery;


            return View(pagedProducts);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            ViewBag.Categories = await categories.GetAllAsync();
            if (id==0)
            {
                ViewBag.Operation = "Add";
                return View(new Product());
            }
            else
            {
                Product product = await products.GetByIdAsync(id, new QueryOptions<Product>());
                ViewBag.Operation = "Edit";
                return View(product);
            }
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddEdit(Product product, int catId)
        {
            ViewBag.Categories = await categories.GetAllAsync();
            if (ModelState.IsValid)
            {
                // Add Operation
                if(product.ProductId == 0)
                {
                    product.CategoryId = catId;
                    await products.AddAsync(product);
                }

                // Edit Opertaion
                else
                {
                    var existingProduct = await products.GetByIdAsync(product.ProductId, new QueryOptions<Product>());

                    if (existingProduct == null)
                    {
                        ModelState.AddModelError("", "Product not found");
                        return View(product);
                    }

                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.ImageUrl = product.ImageUrl;
                    existingProduct.CategoryId = catId;

                    try
                    {
                        await products.UpdateAsync(existingProduct);
                        return RedirectToAction("ViewProduct", "Product", new { id = existingProduct.ProductId });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error: {ex.GetBaseException().Message}");
                        return View(product);
                    }
                }
            }
            return RedirectToAction("Index", "Product");
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Delete Product
            try
            {
                await products.DeleteAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deleting product: {ex.GetBaseException().Message}");
            }


            return RedirectToAction("Index", "Product");
        }

       public async Task<IActionResult> ViewProduct(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index", "Product");
            }

            var product = await products.GetByIdAsync(id, new QueryOptions<Product>());

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Listings = await priceListings.GetAllByIdAsync<int>(id, "ProductId", new QueryOptions<PriceListing>
            {
                OrderBy = listing => listing.Price // Order by Price in ascending order (cheapest first)
            });

            foreach (var listing in ViewBag.Listings)
            {
                listing.Vendor = await vendors.GetByIdAsync(listing.VendorId, new QueryOptions<Vendor>());
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && await _userManager.IsInRoleAsync(user, "User"))
                {
                    // Update Viewing Histories------------------------------------------------------------------------------------------------------------------------------------------
                    try
                    {
                        var existingEntity = await userViewingHistory.GetByIdAsync(user.Id, id, new QueryOptions<UserViewingHistory>());
                        if (existingEntity != null)
                        {
                            existingEntity.LastViewed = DateTime.Now;
                            await userViewingHistory.UpdateAsync(existingEntity);
                        }
                        else
                        {
                            var newEntity = new UserViewingHistory
                            {
                                ProductId = id,
                                LastViewed = DateTime.Now,
                                UserId = user.Id
                            };
                            await userViewingHistory.AddAsync(newEntity);
                        }
                    }
                    catch (Exception ex)
                    {

                        ModelState.AddModelError("", $"Error updating viewing history: {ex.GetBaseException().Message}");
                    }

                    // Fetch viewing history ordered by the oldest first
                    var viewingHistories = await userViewingHistory.GetAllByIdAsync(user.Id, "UserId", new QueryOptions<UserViewingHistory> { OrderBy = e => e.LastViewed });
                    List<UserViewingHistory> viewingHistoriesList = viewingHistories.ToList();

                    if (viewingHistoriesList.Count > 20)
                    {
                        // Delete extra entries beyond 20
                        while (viewingHistoriesList.Count > 20)
                        {
                            try
                            {
                                var entityToDelete = viewingHistoriesList[0];
                                await userViewingHistory.DeleteAsync(entityToDelete); // Delete the oldest entry
                                viewingHistoriesList.RemoveAt(0); // Remove the entity from the list
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


                    // Check if on Wishlist ------------------------------------------------------------------------------------------------------------------------------------------
                    var existingWishlistItem = await userWishlists.GetByIdAsync(user.Id, id, new QueryOptions<UserWishList>());
                    ViewData["OnWishlist"] = (existingWishlistItem == null) ? "False" : "True";

                }
            }

            return View(product);
        }

        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateWishList(int prodId) // If in wishlist delete if not in wishlist add to wishlist
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // Find if product is not in wishlist
                var existingEntity = await userWishlists.GetByIdAsync(user.Id, prodId, new QueryOptions<UserWishList>());

                // Exists so delete from wishlist
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
                // Add to wishlist
                else
                {
                    UserWishList newWishlistItem = new UserWishList{ ProductId = prodId, UserId = user.Id };
                    await userWishlists.AddAsync(newWishlistItem);
                }
            }

            return RedirectToAction("ViewProduct", "Product", new { id = prodId });
        }




        // ------------------------------------------- Helper Methods -------------------------------------------------------------------------------------------------------------------------------------------------------------
        public List<Product> SetupPagination(IEnumerable<Product> allProducts, int pageNumber)
        {
            int pageSize = 12; // Number of products per page

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = (int)Math.Ceiling(allProducts.Count() / (double)pageSize);

            return allProducts
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
        }

    }
}
