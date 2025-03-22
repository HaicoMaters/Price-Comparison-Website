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
                if (id == 0)
                {
                    ViewBag.Operation = "Add";
                    return View(new Vendor());
                }

                var vendor = await _vendorService.GetVendorByIdAsync(id);
                if (vendor == null)
                    return NotFound(new { error = "Vendor not found" });

                ViewBag.Operation = "Edit";
                return View(vendor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading vendor for editing. VendorId: {VendorId}", id);
                return StatusCode(500, new { error = "An error occurred while loading vendor", details = ex.Message });
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
                    return BadRequest(new { error = "Invalid model state", details = ModelState });

                // Add Operation
                if (vendor.VendorId == 0)
                {
                    try
                    {
                        await _vendorService.AddVendorAsync(vendor);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while adding vendor. Vendor: {@Vendor}", vendor);
                        return BadRequest(new { error = "Failed to add vendor", details = ex.Message });
                    }
                }

                // Edit Operation
                var existingVendor = await _vendorService.GetVendorByIdAsync(vendor.VendorId);
                if (existingVendor == null)
                    return NotFound(new { error = "Vendor not found" });

                try
                {
                    existingVendor.VendorUrl = vendor.VendorUrl;
                    existingVendor.VendorLogoUrl = vendor.VendorLogoUrl;
                    existingVendor.Name = vendor.Name;

                    await _vendorService.UpdateVendorAsync(existingVendor);
                    return RedirectToAction("ViewVendor", new { id = existingVendor.VendorId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating vendor. Vendor: {@Vendor}", vendor);
                    return BadRequest(new { error = "Failed to update vendor", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding or editing vendor. Vendor: {@Vendor}", vendor);
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
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
