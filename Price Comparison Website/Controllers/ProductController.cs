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
        private readonly ILogger<ProductController> _logger;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager, ILogger<ProductController> logger)
        {
            products = new Repository<Product>(context);
            categories = new Repository<Category>(context);
            priceListings = new Repository<PriceListing>(context);
            vendors = new Repository<Vendor>(context);
            userViewingHistory = new Repository<UserViewingHistory>(context);
            userWishlists = new Repository<UserWishList>(context);
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int catId = 0, string searchQuery = "")
        {
            try
            {
                // Get categories for filtering
                ViewBag.Categories = await categories.GetAllAsync();
                IEnumerable<Product> allProducts;

                try
                {
                    // Search All Categories or by CategoryId
                    if (catId == 0)
                    {
                        allProducts = await products.GetAllAsync();
                    }
                    else
                    {
                        allProducts = await products.GetAllByIdAsync(catId, "CategoryId", new QueryOptions<Product>
                        {
                            Includes = "Category",
                            Where = p => p.CategoryId == catId
                        });
                    }

                    // Apply search filter if provided
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        allProducts = allProducts.Where(p =>
                            (p.Name != null && p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                            (p.Description != null && p.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)))
                            .ToList();
                    }

                    // Paginate products
                    var pagedProducts = SetupPagination(allProducts, pageNumber);

                    // Handle User Specific Data for wishlist
                    if (User.Identity.IsAuthenticated)
                    {
                        var user = await _userManager.GetUserAsync(User);
                        if (user != null && await _userManager.IsInRoleAsync(user, "User"))
                        {
                            List<bool> onWishlist = new List<bool>();
                            foreach (var product in pagedProducts)
                            {
                                var existingEntity = await userWishlists.GetByIdAsync(user.Id, product.ProductId, new QueryOptions<UserWishList>());
                                onWishlist.Add(existingEntity != null);
                            }
                            ViewBag.OnWishlist = onWishlist;
                        }
                    }

                    ViewData["CategoryId"] = catId;
                    ViewData["SearchQuery"] = searchQuery;

                    return View(pagedProducts);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Invalid operation while fetching products. CategoryId: {CategoryId}, SearchQuery: {SearchQuery}",
                        catId, searchQuery);
                    return BadRequest(new { error = "Invalid operation while fetching products", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products. CategoryId: {CategoryId}, SearchQuery: {SearchQuery}",
                    catId, searchQuery);
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            try
            {
                ViewBag.Categories = await categories.GetAllAsync();

                if (id == 0)
                {
                    ViewBag.Operation = "Add";
                    return View(new Product());
                }

                var product = await products.GetByIdAsync(id, new QueryOptions<Product>());
                if (product == null)
                    return NotFound(new { error = "Product not found" });

                ViewBag.Operation = "Edit";
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading product. ProductId: {ProductId}", id);
                return StatusCode(500, new { error = "An error occurred while loading product", details = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddEdit(Product product, int catId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = "Invalid model state", details = ModelState });

                // Add Operation
                if (product.ProductId == 0)
                {
                    try
                    {
                        product.CategoryId = catId;
                        await products.AddAsync(product);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to add product. Product: {Product}", product);
                        return BadRequest(new { error = "Failed to add product", details = ex.Message });
                    }
                }

                // Edit Operation
                var existingProduct = await products.GetByIdAsync(product.ProductId, new QueryOptions<Product>());
                if (existingProduct == null)
                    return NotFound(new { error = "Product not found" });

                try
                {
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.CategoryId = catId;

                    await products.UpdateAsync(existingProduct);
                    return RedirectToAction("ViewProduct", new { id = existingProduct.ProductId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update product. Product: {Product}", product);
                    return BadRequest(new { error = "Failed to update product", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding/editing product. Product: {Product}", product);
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
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
                _logger.LogWarning(ex, "Invalid operation while deleting product. ProductId: {ProductId}", id);
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product. ProductId: {ProductId}", id);
                ModelState.AddModelError("", $"Error deleting product: {ex.GetBaseException().Message}");
            }


            return RedirectToAction("Index", "Product");
        }

        public async Task<IActionResult> ViewProduct(int id)
        {
            try
            {
                if (id == 0)
                    return RedirectToAction("Index", "Product");

                var product = await products.GetByIdAsync(id, new QueryOptions<Product>());
                if (product == null)
                    return NotFound(new { error = "Product not found" });

                try
                {
                    // Get price listings
                    ViewBag.Listings = await priceListings.GetAllByIdAsync<int>(id, "ProductId", new QueryOptions<PriceListing>
                    {
                        OrderBy = listing => listing.DiscountedPrice // Order by Price in ascending order (cheapest first) discounted price is set to price if not discounted
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
                            // Update Viewing Histories
                            try
                            {
                                await UpdateViewingHistory(user.Id, id);
                                await CleanupViewingHistory(user.Id);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error managing viewing history. UserId: {UserId}, ProductId: {ProductId}", user.Id, id);
                            }

                            // Check if on Wishlist
                            try
                            {
                                var existingWishlistItem = await userWishlists.GetByIdAsync(user.Id, id, new QueryOptions<UserWishList>());
                                ViewData["OnWishlist"] = (existingWishlistItem == null) ? "False" : "True";
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error checking wishlist status. UserId: {UserId}, ProductId: {ProductId}", user.Id, id);
                                ViewData["OnWishlist"] = "False";
                            }
                        }
                    }

                    return View(product);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Invalid operation while loading product details. ProductId: {ProductId}", id);
                    return BadRequest(new { error = "Invalid operation while loading product details", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading product details. ProductId: {ProductId}", id);
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }

        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> UpdateWishList(int prodId)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { error = "User not authenticated" });

            try
            {
                // Find if product is in wishlist
                var existingEntity = await userWishlists.GetByIdAsync(user.Id, prodId, new QueryOptions<UserWishList>());

                if (existingEntity != null)
                {
                    // Exists so delete from wishlist
                    try
                    {
                        await userWishlists.DeleteAsync(existingEntity);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to remove from wishlist. UserId: {UserId}, ProductId: {ProductId}", user.Id, prodId);
                        return BadRequest(new { error = "Failed to remove from wishlist", details = ex.Message });
                    }
                }
                else
                {
                    // Add to wishlist
                    var existingProd = await products.GetByIdAsync(prodId, new QueryOptions<Product>());
                    if (existingProd == null)
                        return NotFound(new { error = "Product not found" });

                    try
                    {
                        var newWishlistItem = new UserWishList
                        {
                            ProductId = prodId,
                            UserId = user.Id,
                            LastCheapestPrice = existingProd.CheapestPrice
                        };
                        await userWishlists.AddAsync(newWishlistItem);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to add to wishlist. UserId: {UserId}, ProductId: {ProductId}", user.Id, prodId);
                        return BadRequest(new { error = "Failed to add to wishlist", details = ex.Message });
                    }
                }

                return RedirectToAction("ViewProduct", "Product", new { id = prodId });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while updating wishlist. UserId: {UserId}, ProductId: {ProductId}", user.Id, prodId);
                return BadRequest(new { error = "Invalid operation while updating wishlist", details = ex.Message });
            }
        }

        //  Helper Methods -------------------------------------------------------------------------------------------------------------------------------------------------------------
        private async Task UpdateViewingHistory(string userId, int productId)
        {
            var existingEntity = await userViewingHistory.GetByIdAsync(userId, productId, new QueryOptions<UserViewingHistory>());
            if (existingEntity != null)
            {
                existingEntity.LastViewed = DateTime.Now;
                await userViewingHistory.UpdateAsync(existingEntity);
            }
            else
            {
                var newEntity = new UserViewingHistory
                {
                    ProductId = productId,
                    LastViewed = DateTime.Now,
                    UserId = userId
                };
                await userViewingHistory.AddAsync(newEntity);
            }
        }

        private async Task CleanupViewingHistory(string userId)
        {
            var viewingHistories = await userViewingHistory.GetAllByIdAsync(userId, "UserId",
                new QueryOptions<UserViewingHistory> { OrderBy = e => e.LastViewed });
            var viewingHistoriesList = viewingHistories.ToList();

            while (viewingHistoriesList.Count > 20)
            {
                var entityToDelete = viewingHistoriesList[0];
                await userViewingHistory.DeleteAsync(entityToDelete);
                viewingHistoriesList.RemoveAt(0);
            }
        }

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
