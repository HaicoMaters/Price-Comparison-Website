﻿using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using PriceComparisonWebsite.Data;
using PriceComparisonWebsite.Models;
using PriceComparisonWebsite.Services.Interfaces;

namespace PriceComparisonWebsite.Controllers
{
    /// <summary>
    /// Controller for managing vendors and their related operations
    /// </summary>
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

        /// <summary>
        /// Displays a paginated list of vendors with optional search functionality
        /// </summary>
        /// <param name="pageNumber">The page number to display</param>
        /// <param name="searchQuery">Optional search term to filter vendors</param>
        /// <returns>The index view with filtered and paginated vendors</returns>
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

        /// <summary>
        /// Displays the add/edit form for a vendor
        /// </summary>
        /// <param name="id">The ID of the vendor to edit (0 for new vendor)</param>
        /// <returns>The add/edit view with the vendor data</returns>
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

        /// <summary>
        /// Processes the add/edit form submission for a vendor
        /// </summary>
        /// <param name="vendor">The vendor data from the form</param>
        /// <returns>Redirects to the vendor view on success, or returns the form with errors</returns>
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

        /// <summary>
        /// Displays detailed information about a specific vendor
        /// </summary>
        /// <param name="id">The ID of the vendor to view</param>
        /// <returns>The vendor view with detailed vendor information and listings</returns>
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

        /// <summary>
        /// Deletes a vendor
        /// </summary>
        /// <param name="id">The ID of the vendor to delete</param>
        /// <returns>Redirects to the index view on success</returns>
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
