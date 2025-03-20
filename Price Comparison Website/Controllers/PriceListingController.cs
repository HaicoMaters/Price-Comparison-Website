using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;
using Price_Comparison_Website.Services;
using Price_Comparison_Website.Services.Interfaces;

namespace Price_Comparison_Website.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PriceListingController : Controller
	{
		private readonly IPriceListingService _priceListingService;
        private readonly IVendorService _vendorService;
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<PriceListingController> _logger;

        public PriceListingController(IPriceListingService priceListingService,
            IVendorService vendorService,
            IProductService productService,
            INotificationService notificationService,
            ILogger<PriceListingController> logger)
        {
            _priceListingService = priceListingService;
            _vendorService = vendorService;
            _productService = productService;
            _notificationService = notificationService;
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

					await UpdateCheapestPrice(priceListing.ProductId, priceListing.DiscountedPrice); // Update cheapest price after adding/editing to send notifications if price drops
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


		// Helper Methods -------------------------------------------------------
		public async Task UpdateCheapestPrice(int productId, decimal newPrice)
		{
			try
			{
				var existingProduct = await _productService.GetProductById(productId, new QueryOptions<Product>());
				if (existingProduct == null)
					throw new InvalidOperationException("Product not found");

				decimal oldPrice = existingProduct.CheapestPrice;
				if (newPrice < existingProduct.CheapestPrice || existingProduct.CheapestPrice == 0)
				{
					// Send Notification to Users with item on wishlist if price drops
					await _notificationService.CreateProductPriceDropNotifications(productId, existingProduct.Name, newPrice, oldPrice);
				}
				await _productService.RecalculateCheapestPrice(productId); // Recalculate cheapest price after updating
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to update cheapest price. ProductId: {ProductId}", productId);
				throw new InvalidOperationException("Failed to update cheapest price", ex);
			}
		}
    }
}
