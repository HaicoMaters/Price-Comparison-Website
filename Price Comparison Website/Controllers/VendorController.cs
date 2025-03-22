using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;
using Price_Comparison_Website.Services.Interfaces;

namespace Price_Comparison_Website.Controllers
{
    public class VendorController : Controller
    {
        private readonly IVendorService _vendorService;
        private readonly IProductService _productService;
        private readonly IPriceListingService _priceListingService;
        private readonly ILogger<VendorController> _logger;

        public VendorController(IVendorService vendorService, IProductService productService, IPriceListingService priceListingService, ILogger<VendorController> logger)
        {
            _logger = logger;
            _vendorService = vendorService;
            _priceListingService = priceListingService;
            _productService = productService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string searchQuery = "")
        {
            try
            {
                int pageSize = 20; // Number of products per page
                var allVendors = await _vendorService.GetAllVendorsAsync();

                if (!string.IsNullOrEmpty(searchQuery)) // Search functionality
                {
                    allVendors = allVendors.Where(p => (p.Name != null &&
                        p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))).ToList();
                }

                // Paginate the products
                var pagedVendors = _vendorService.SetupPagination(allVendors, pageNumber, ViewData);

                // Set ViewData
                ViewData["SearchQuery"] = searchQuery;

                return View(pagedVendors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching vendors. PageNumber: {PageNumber}, SearchQuery: {SearchQuery}",
                    pageNumber, searchQuery);
                return StatusCode(500, new { error = "An error occurred while fetching vendors", details = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            try
            {
                if (id == 0) // Add Operation
                {
                    ViewBag.Operation = "Add";
                    return View(new Vendor());
                }

                var vendor = await _vendorService.GetVendorByIdAsync(id);
                if (vendor == null)
                    return NotFound();

                ViewBag.Operation = "Edit"; // Edit Operation
                return View(vendor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading vendor for editing. VendorId: {VendorId}", id);
                return StatusCode(500);
            }
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddEdit(Vendor vendor)
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
                    ViewBag.Operation = vendor.VendorId == 0 ? "Add" : "Edit";
                    return View(vendor);
                }

                try
                {
                    if (vendor.VendorId == 0) // Add Operation
                    {
                        await _vendorService.AddVendorAsync(vendor);
                        return RedirectToAction("ViewVendor", new { id = vendor.VendorId });
                    }

                    // Edit Operation
                    await _vendorService.UpdateVendorAsync(vendor);
                    return RedirectToAction("ViewVendor", new { id = vendor.VendorId });

                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Error occurred while adding vendor. Vendor: {@Vendor}", vendor);
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding or editing vendor. Vendor: {@Vendor}", vendor);
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> ViewVendor(int id)
        {
            try
            {
                if (id == 0)
                    return RedirectToAction("Index");

                var vendor = await _vendorService.GetVendorByIdAsync(id);
                if (vendor == null)
                    return NotFound(new { error = "Vendor not found" });

                try
                {
                    var listings = await _priceListingService.GetPriceListingsByVendorId(id, new QueryOptions<PriceListing>());
                    ViewBag.Listings = listings.OrderByDescending(e => e.DateListed);

                    foreach (var listing in ViewBag.Listings)
                    {
                        listing.Product = await _productService.GetProductById(listing.ProductId, new QueryOptions<Product>());
                    }

                    return View(vendor);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Error occurred while loading vendor details. VendorId: {VendorId}", id);
                    return BadRequest(new { error = "Failed to load vendor details", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while viewing vendor. VendorId: {VendorId}", id);
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var vendor = await _vendorService.GetVendorByIdAsync(id);
                if (vendor == null)
                    return NotFound(new { error = "Vendor not found" });

                try
                {
                    await _vendorService.DeleteVendorAsync(id);
                    return RedirectToAction("Index");
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Error occurred while deleting vendor. VendorId: {VendorId}", id);
                    return BadRequest(new { error = "Failed to delete vendor", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting vendor. VendorId: {VendorId}", id);
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }
    }
}
