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
using Price_Comparison_Website.Services.Implementations;
using Price_Comparison_Website.Services.Interfaces;

namespace Price_Comparison_Website.Controllers
{
    public class ProductController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IPriceListingService _priceListingService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            ICategoryService categoryService,
            IProductService productService,
            IVendorService vendorService,
            IUserService userService,
            IPriceListingService priceListingService,
            IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager, 
            ILogger<ProductController> logger)
        {
            _categoryService = categoryService;
            _productService = productService;
            _vendorService = vendorService;
            _userService = userService;
            _priceListingService = priceListingService;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int catId = 0, string searchQuery = "")
        {
            try
            {
                // Get categories for filtering
                ViewBag.Categories = await _categoryService.GetAllCategories();
                IEnumerable<Product> allProducts;

                try
                {
                    // Search All Categories or by CategoryId
                    if (catId == 0)
                    {
                        allProducts = await _productService.GetAllProducts();
                    }
                    else
                    {
                        allProducts = await _productService.GetProductsByCategoryId(catId);
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
                                var existingEntity = await _userService.GetUserWishListItemById(user.Id, product.ProductId);
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
                ViewBag.Categories = await _categoryService.GetAllCategories();

                if (id == 0)
                {
                    ViewBag.Operation = "Add";
                    return View(new Product());
                }

                var product = await _productService.GetProductById(id);
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
        public async Task<IActionResult> AddEdit(Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid:");
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            _logger.LogWarning($"Error: {error.ErrorMessage}");
                        }
                    }
                    ViewBag.Categories = await _categoryService.GetAllCategories();
                    ViewBag.Operation = product.ProductId == 0 ? "Add" : "Edit";
                    return View(product);
                }

                // Add Operation
                if (product.ProductId == 0)
                {
                    try
                    {
                        await _productService.AddProduct(product);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to add product. Product: {Product}", product);
                        return BadRequest(new { error = "Failed to add product", details = ex.Message });
                    }
                }

                // Edit Operation

                try
                {
                    var prod = await _productService.UpdateProduct(product, product.CategoryId);
                    return RedirectToAction("ViewProduct", new { id = prod.ProductId });
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
                await _productService.DeleteProduct(id);
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

                var product = await _productService.GetProductById(id);
                if (product == null)
                    return NotFound(new { error = "Product not found" });

                try
                {
                    // Get price listings
                    var listings = await _priceListingService.GetPriceListingsByProductId(id);
                    ViewBag.Listings = listings.OrderBy(listing => listing.DiscountedPrice);

                    foreach (var listing in ViewBag.Listings)
                    {
                        listing.Vendor = await _vendorService.GetVendorByIdAsync(listing.VendorId);
                    }

                    if (User.Identity.IsAuthenticated)
                    {
                        var user = await _userManager.GetUserAsync(User);
                        if (user != null && await _userManager.IsInRoleAsync(user, "User"))
                        {
                            // Update Viewing Histories
                            try
                            {
                                await _userService.UpdateViewingHistory(user.Id, id);
                                await _userService.CleanupViewingHistory(user.Id);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error managing viewing history. UserId: {UserId}, ProductId: {ProductId}", user.Id, id);
                            }

                            // Check if on Wishlist
                            try
                            {
                                var existingWishlistItem = await _userService.GetUserWishListItemById(user.Id, id);
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
                var existingEntity = await _userService.GetUserWishListItemById(user.Id, prodId);

                if (existingEntity != null)
                {
                    // Exists so delete from wishlist
                    try
                    {
                        await _userService.RemoveFromWishlist(prodId, user.Id);
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
                    var existingProd = await _productService.GetProductById(prodId);
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
                        await _userService.AddWishlistItem(newWishlistItem);
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
