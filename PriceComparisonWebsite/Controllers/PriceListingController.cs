﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceComparisonWebsite.Data;
using PriceComparisonWebsite.Models;
using PriceComparisonWebsite.Services;
using PriceComparisonWebsite.Services.Interfaces;

namespace PriceComparisonWebsite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PriceListingController : Controller
	{
		private readonly IPriceListingService _priceListingService;
        private readonly IVendorService _vendorService;
        private readonly IProductService _productService;
        private readonly ILogger<PriceListingController> _logger;

        public PriceListingController(IPriceListingService priceListingService,
            IVendorService vendorService,
            IProductService productService,
            ILogger<PriceListingController> logger)
        {
            _priceListingService = priceListingService;
            _vendorService = vendorService;
            _productService = productService;
            _logger = logger;
        }

		public IActionResult Index()
		{
			return RedirectToAction("Index", "Home");
		}

        [HttpGet]
		public async Task<IActionResult> AddEdit(int id, int prodId)
		{
			try
			{
				// Get product first
				var product = await _productService.GetProductById(prodId, new QueryOptions<Product>());

				ViewBag.Product = product;
				ViewBag.Vendors = await _vendorService.GetAllVendorsAsync();

				if (product == null)
				{
					return NotFound();
				}

				if (id == 0)
				{
					ViewBag.Operation = "Add";
					return View(new PriceListing() { ProductId = prodId });
				}
				
				var priceListing = await _priceListingService.GetPriceListingById(id, new QueryOptions<PriceListing>());

				if (priceListing == null)
				{
					return NotFound();
				}

				ViewBag.Operation = "Edit";
				return View(priceListing);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while loading price listing. ListingId: {ListingId}, ProductId: {ProductId}", 
					id, prodId);
				return StatusCode(500);
			}
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> AddEdit(PriceListing priceListing)
		{
			try
			{
				if (!ModelState.IsValid)
					return View(priceListing);

				try
				{
					// Ensure discounted price is set correctly
					if (!Request.Form.ContainsKey("IsDiscounted") || priceListing.DiscountedPrice > priceListing.Price)
					{
						priceListing.DiscountedPrice = priceListing.Price;
					}

					await _productService.RecalculateCheapestPrice(priceListing.ProductId); // Update cheapest price before adding/editing in case of price change in other parts of the system for notifs
					priceListing.DateListed = DateTime.Now;
					// Add Operation
					if (priceListing.PriceListingId == 0)
					{
						await _priceListingService.AddPriceListing(priceListing);
					}
					else // Edit Operation
					{
						await _priceListingService.UpdatePriceListing(priceListing);
					}

					await _priceListingService.UpdateCheapestPrice(priceListing.ProductId, priceListing.DiscountedPrice); // Update cheapest price after adding/editing to send notifications if price drops
					return RedirectToAction("ViewProduct", "Product", new { id = priceListing.ProductId });
				}
				catch (InvalidOperationException ex)
				{
					return BadRequest();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while adding/editing price listing. ListingId: {ListingId}, ProductId: {ProductId}", 
					priceListing.PriceListingId, priceListing.ProductId);
				return StatusCode(500);
			}
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existingPriceListing = await _priceListingService.GetPriceListingById(id, new QueryOptions<PriceListing>());
                if (existingPriceListing == null)
                    return NotFound();
				
				int prodId = existingPriceListing.ProductId;

                try
                {
                    await _priceListingService.DeletePriceListing(id);
					await _productService.RecalculateCheapestPrice(prodId); // Update cheapest price after deletion
                    return RedirectToAction("ViewProduct", "Product", new { id = existingPriceListing.ProductId });
                }
                catch (InvalidOperationException ex)
                {
					_logger.LogError(ex, "Failed to delete price listing. ListingId: {ListingId}", id);
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
				_logger.LogError(ex, "Error occurred while deleting price listing. ListingId: {ListingId}", id);
                return StatusCode(500);
            }
        }
    }
}
