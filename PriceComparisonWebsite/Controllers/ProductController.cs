using System.Drawing.Printing;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PriceComparisonWebsite.Data;
using PriceComparisonWebsite.Models;
using PriceComparisonWebsite.Services.Implementations;
using PriceComparisonWebsite.Services.Interfaces;

namespace PriceComparisonWebsite.Controllers
{
    public class ProductController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IPriceListingService _priceListingService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            ICategoryService categoryService,
            IProductService productService,
            IVendorService vendorService,
            IUserService userService,
            IPriceListingService priceListingService,
            UserManager<ApplicationUser> userManager,
            ILogger<ProductController> logger)
        {
            _categoryService = categoryService;
            _productService = productService;
            _vendorService = vendorService;
            _userService = userService;
            _priceListingService = priceListingService;
            _userManager = userManager;
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
                        allProducts = await _productService.GetProductsByCategoryId(catId, new QueryOptions<Product>());
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
                    var pagedProducts = _productService.SetupPagination(allProducts, pageNumber, ViewData);

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
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products. CategoryId: {CategoryId}, SearchQuery: {SearchQuery}",
                    catId, searchQuery);
                return StatusCode(500);
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

                var product = await _productService.GetProductById(id, new QueryOptions<Product>());
                if (product == null)
                    return NotFound();

                ViewBag.Operation = "Edit";
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading product. ProductId: {ProductId}", id);
                return StatusCode(500);
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

                try
                {
                    if (product.ProductId == 0) // Add Operation
                    {
                        await _productService.AddProduct(product);
                        return RedirectToAction("ViewProduct", new { id = product.ProductId });
                    }

                    // Edit Operation
                    await _productService.UpdateProduct(product);
                    return RedirectToAction("ViewProduct", new { id = product.ProductId });

                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Failed to add product. Product: {Product}", product);
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding/editing product. Product: {Product}", product);
                return StatusCode(500);
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
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product. ProductId: {ProductId}", id);
                return StatusCode(500);
            }

            return RedirectToAction("Index", "Product");
        }

        public async Task<IActionResult> ViewProduct(int id)
        {

            if (id == 0)
                return RedirectToAction("Index", "Product");

            try
            {
                var product = await _productService.GetProductById(id, new QueryOptions<Product>());

                if (product == null)
                    return NotFound();

                // Get price listings
                ViewBag.Listings = await _priceListingService.GetPriceListingsByProductId(id, new QueryOptions<PriceListing> { Includes = "Vendor", OrderBy = l => l.DiscountedPrice });

                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);

                    if (user != null && await _userManager.IsInRoleAsync(user, "User"))
                    {
                        // Update Viewing History and Cleanup
                        await _userService.UpdateViewingHistory(user.Id, id);
                        await _userService.CleanupViewingHistory(user.Id);

                        // Check if on Wishlist
                        var wishlistItem = await _userService.GetUserWishListItemById(user.Id, id);
                        ViewData["OnWishlist"] = wishlistItem != null;
                    }
                }

                return View(product);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Invalid operation while viewing product. ProductId: {ProductId}", id);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading product details. ProductId: {ProductId}", id);
                return StatusCode(500);
            }
        }


        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> UpdateWishList(int prodId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            try
            {
                bool? result = await _userService.UpdateUserWishlist(user.Id, prodId);

                if (result == null)
                    return NotFound();

                if (result == true)
                    return RedirectToAction("ViewProduct", "Product", new { id = prodId });

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating wishlist. UserId: {UserId}, ProductId: {ProductId}", user.Id, prodId);
                return StatusCode(500);
            }
        }

    }
}
