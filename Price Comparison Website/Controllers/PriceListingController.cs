using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;
using Price_Comparison_Website.Services;

namespace Price_Comparison_Website.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PriceListingController : Controller
	{
		private Repository<PriceListing> priceListings;
		private Repository<Vendor> vendors;
		private Repository<Product> products;
		private NotificationService notificationService;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public PriceListingController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			priceListings = new Repository<PriceListing>(context);
			vendors = new Repository<Vendor>(context);
			products = new Repository<Product>(context);
			notificationService = new NotificationService(context);
			_webHostEnvironment = webHostEnvironment;
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
				var product = await products.GetByIdAsync(prodId, new QueryOptions<Product>
				{
					Where = p => p.ProductId == prodId
				});
				if (product == null)
					return NotFound(new { error = "Product not found" });

				ViewBag.Product = product;
				ViewBag.Vendors = await vendors.GetAllAsync();

				if (id == 0)
				{
					ViewBag.Operation = "Add";
					return View(new PriceListing() { ProductId = prodId });
				}
				
				var priceListing = await priceListings.GetByIdAsync(id, new QueryOptions<PriceListing>
				{
					Includes = "Product",
					Where = pl => pl.PriceListingId == id
				});
				if (priceListing == null)
					return NotFound(new { error = "Price listing not found" });

				ViewBag.Operation = "Edit";
				return View(priceListing);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = "An error occurred while loading the price listing", details = ex.Message });
			}
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> AddEdit(PriceListing priceListing)
		{
			try
			{
				ViewBag.Vendors = await vendors.GetAllAsync();

				if (!ModelState.IsValid)
					return View(priceListing);

				try
				{
					// Ensure discounted price is set correctly
					if (!Request.Form.ContainsKey("IsDiscounted") || priceListing.DiscountedPrice > priceListing.Price)
					{
						priceListing.DiscountedPrice = priceListing.Price;
					}

					await RecalculateCheapestPrice(priceListing.ProductId); // Update cheapest price before adding/editing in case of price change in other parts of the system for notifs

					// Add Operation
					if (priceListing.PriceListingId == 0)
					{
						priceListing.DateListed = DateTime.Now;
						await priceListings.AddAsync(priceListing);
					}
					else // Edit Operation
					{
						await HandleExistingPriceListing(priceListing);
					}

					await UpdateCheapestPrice(priceListing.ProductId, priceListing.DiscountedPrice); // Update cheapest price after adding/editing to send notifications if price drops
					return RedirectToAction("ViewProduct", "Product", new { id = priceListing.ProductId });
				}
				catch (InvalidOperationException ex)
				{
					ModelState.AddModelError("", ex.Message);
					return View(priceListing);
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"Error: {ex.GetBaseException().Message}");
					return View(priceListing);
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
			}
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existingPriceListing = await priceListings.GetByIdAsync(id, new QueryOptions<PriceListing>());
                if (existingPriceListing == null)
                    return NotFound(new { error = "Price listing not found" });
				
				int prodId = existingPriceListing.ProductId;

                try
                {
                    await priceListings.DeleteAsync(id);
					await RecalculateCheapestPrice(prodId); // Update cheapest price after deletion
                    return RedirectToAction("ViewProduct", "Product", new { id = existingPriceListing.ProductId });
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { error = "Failed to delete price listing", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }


		// Helper Methods -------------------------------------------------------
		private async Task HandleExistingPriceListing(PriceListing priceListing)
		{
			var existingPriceListing = await priceListings.GetByIdAsync(priceListing.PriceListingId, new QueryOptions<PriceListing>());
			if (existingPriceListing == null)
				throw new InvalidOperationException("Listing not found");

			existingPriceListing.Price = priceListing.Price;
			existingPriceListing.DiscountedPrice = priceListing.DiscountedPrice;
			existingPriceListing.PurchaseUrl = priceListing.PurchaseUrl;
			existingPriceListing.VendorId = priceListing.VendorId;
			existingPriceListing.DateListed = DateTime.Now;

			await priceListings.UpdateAsync(existingPriceListing);
		}

		public async Task UpdateCheapestPrice(int productId, decimal newPrice)
		{
			try
			{
				var existingProduct = await products.GetByIdAsync(productId, new QueryOptions<Product>());
				if (existingProduct == null)
					throw new InvalidOperationException("Product not found");

				decimal oldPrice = existingProduct.CheapestPrice;
				if (newPrice < existingProduct.CheapestPrice || existingProduct.CheapestPrice == 0)
				{
					// Send Notification to Users with item on wishlist if price drops
					await notificationService.CreateProductPriceDropNotifications(productId, existingProduct.Name, newPrice, oldPrice);
				}
				await RecalculateCheapestPrice(productId); // Recalculate cheapest price after updating
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("Failed to update cheapest price", ex);
			}
		}

		public async Task RecalculateCheapestPrice(int productId){
			try
			{
				var product = await products.GetByIdAsync(productId, new QueryOptions<Product>());
				if (product == null)
					throw new InvalidOperationException("Product not found");

				var listings = await priceListings.GetAllByIdAsync(productId, "ProductId", new QueryOptions<PriceListing>());
				if (listings == null)
					throw new InvalidOperationException("No listings found for product");

				decimal cheapestPrice = listings.Min(l => l.DiscountedPrice);
				product.CheapestPrice = cheapestPrice;
				await products.UpdateAsync(product);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("Failed to recalculate cheapest price", ex);
			}
		}

    }
}
