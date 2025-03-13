using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Controllers
{
    public class VendorController : Controller
    {
        private Repository<Vendor> vendors;
        private Repository<PriceListing> priceListings;
        private Repository<Product> products;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VendorController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            vendors = new Repository<Vendor>(context);
            priceListings = new Repository<PriceListing>(context);
            products = new Repository<Product>(context);
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string searchQuery = "")
        {
            try
            {
                int pageSize = 20; // Number of products per page
                var allVendors = await vendors.GetAllAsync();

                if (!string.IsNullOrEmpty(searchQuery)) // Search functionality
                {
                    allVendors = allVendors.Where(p => (p.Name != null && 
                        p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))).ToList();
                }

                // Paginate the products
                var pagedVendors = SetupPagination(allVendors, pageNumber);

                // Set ViewData
                ViewData["SearchQuery"] = searchQuery;

                return View(pagedVendors);
            }
            catch (Exception ex)
            {
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

                var vendor = await vendors.GetByIdAsync(id, new QueryOptions<Vendor>());
                if (vendor == null)
                    return NotFound(new { error = "Vendor not found" });

                ViewBag.Operation = "Edit";
                return View(vendor);
            }
            catch (Exception ex)
            {
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
                        await vendors.AddAsync(vendor);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { error = "Failed to add vendor", details = ex.Message });
                    }
                }

                // Edit Operation
                var existingVendor = await vendors.GetByIdAsync(vendor.VendorId, new QueryOptions<Vendor>());
                if (existingVendor == null)
                    return NotFound(new { error = "Vendor not found" });

                try
                {
                    existingVendor.VendorUrl = vendor.VendorUrl;
                    existingVendor.VendorLogoUrl = vendor.VendorLogoUrl;
                    existingVendor.Name = vendor.Name;

                    await vendors.UpdateAsync(existingVendor);
                    return RedirectToAction("ViewVendor", new { id = existingVendor.VendorId });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = "Failed to update vendor", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }

        public async Task<IActionResult> ViewVendor(int id)
        {
            try
            {
                if (id == 0)
                    return RedirectToAction("Index");

                var vendor = await vendors.GetByIdAsync(id, new QueryOptions<Vendor>());
                if (vendor == null)
                    return NotFound(new { error = "Vendor not found" });

                try
                {
                    var listings = await priceListings.GetAllByIdAsync<int>(id, "VendorId", new QueryOptions<PriceListing>());
                    ViewBag.Listings = listings.OrderByDescending(e => e.DateListed);

                    foreach (var listing in ViewBag.Listings)
                    {
                        listing.Product = await products.GetByIdAsync(listing.ProductId, new QueryOptions<Product>());
                    }

                    return View(vendor);
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { error = "Failed to load vendor details", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
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
                var vendor = await vendors.GetByIdAsync(id, new QueryOptions<Vendor>());
                if (vendor == null)
                    return NotFound(new { error = "Vendor not found" });

                try
                {
                    await vendors.DeleteAsync(id);
                    return RedirectToAction("Index");
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { error = "Failed to delete vendor", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }

        // ------------------------------------------- Helper Methods -------------------------------------------------------------------------------------------------------------------------------------------------------------
        private List<Vendor> SetupPagination(IEnumerable<Vendor> allVendors, int pageNumber)
        {
            int pageSize = 12; // Number of products per page

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = (int)Math.Ceiling(allVendors.Count() / (double)pageSize);

            return allVendors
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
        }
    }
}
