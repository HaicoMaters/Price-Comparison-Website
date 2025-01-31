﻿using System.Data;
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
            int pageSize = 20; // Number of products per page
            var allVendors = await vendors.GetAllAsync();

            if (!string.IsNullOrEmpty(searchQuery)) // Seaarch functionality
            {
                allVendors = allVendors.Where(p => (p.Name != null && p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            // Paginate the products
            var pagedVendors = SetupPagination(allVendors, pageNumber);

            // Set ViewData
            ViewData["SearchQuery"] = searchQuery;

            return View(pagedVendors);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            if (id == 0)
            {
                ViewBag.Operation = "Add";
                return View(new Vendor());
            }
            else
            {
                Vendor vendor = await vendors.GetByIdAsync(id, new QueryOptions<Vendor>());
                ViewBag.Operation = "Edit";
                return View(vendor);
            }
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddEdit(Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                // Add Operation
                if (vendor.VendorId == 0)
                {
                    vendors.AddAsync(vendor);
                }
                // Edit Operation
                else
                {
                    var existingVendor = await vendors.GetByIdAsync(vendor.VendorId, new QueryOptions<Vendor>());

                    if (existingVendor == null)
                    {
                        ModelState.AddModelError("", "Vendor not found");
                        return View(vendor);
                    }

                    existingVendor.VendorUrl = vendor.VendorUrl;
                    existingVendor.VendorLogoUrl = vendor.VendorLogoUrl;
                    existingVendor.Name = vendor.Name;

                    try
                    {
                        vendors.UpdateAsync(existingVendor);
                        return RedirectToAction("ViewVendor", "Vendor", new { id = existingVendor.VendorId });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error: {ex.GetBaseException().Message}");
                        return View(vendor);
                    }
                }
            }
            return RedirectToAction("Index", "Vendor");
        }

        public async Task<IActionResult> ViewVendor(int id)
        {
            var listings = await priceListings.GetAllByIdAsync<int>(id, "VendorId", new QueryOptions<PriceListing>());
            ViewBag.Listings = listings.OrderByDescending(e => e.DateListed);
            foreach (PriceListing listing in ViewBag.Listings)
            {
                listing.Product = await products.GetByIdAsync(listing.ProductId, new QueryOptions<Product>());
            }

            if (id == 0)
            {
                return RedirectToAction("Index", "Vendor");
            }

            var vendor = await vendors.GetByIdAsync(id, new QueryOptions<Vendor>());

            return View(vendor);
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Delete Vendor
            try
            {
                await vendors.DeleteAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deleting product: {ex.GetBaseException().Message}");
            }


            return RedirectToAction("Index", "Vendor");
        }

        // ------------------------------------------- Helper Methods -------------------------------------------------------------------------------------------------------------------------------------------------------------
        public List<Vendor> SetupPagination(IEnumerable<Vendor> allVendors, int pageNumber)
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
