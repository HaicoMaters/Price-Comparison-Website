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
			ViewBag.Product = await products.GetByIdAsync(prodId, new QueryOptions<Product>
				{
				Where = p => p.ProductId == prodId
				});
			ViewBag.Vendors = await vendors.GetAllAsync();
			if(id == 0)
			{
				ViewBag.Operation = "Add";
				return View(new PriceListing() { ProductId = prodId });
			}
			
			else
			{
				PriceListing priceListing = await priceListings.GetByIdAsync(id, new QueryOptions<PriceListing>
				{
					Includes = "Product", Where = pl => pl.PriceListingId == id
				});
				ViewBag.Operation = "Edit";
				return View(priceListing);
			}
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> AddEdit(PriceListing priceListing)
		{
			ViewBag.Vendors = await vendors.GetAllAsync();

			if (ModelState.IsValid)
			{
				// Ensure discounted price is set correctly
				if (!Request.Form.ContainsKey("IsDiscounted") || priceListing.DiscountedPrice > priceListing.Price)
				{
					priceListing.DiscountedPrice = priceListing.Price;
				}

				// Add Operation
				if (priceListing.PriceListingId == 0)
				{
					priceListing.DateListed = DateTime.Now;
					await priceListings.AddAsync(priceListing);
				}
				else
				{
					var existingPriceListing = await priceListings.GetByIdAsync(priceListing.PriceListingId, new QueryOptions<PriceListing>());

					if (existingPriceListing == null)
					{
						ModelState.AddModelError("", "Listing not found");
						return View(priceListing);
					}

					existingPriceListing.Price = priceListing.Price;
					existingPriceListing.DiscountedPrice = priceListing.DiscountedPrice;
					existingPriceListing.PurchaseUrl = priceListing.PurchaseUrl;
					existingPriceListing.VendorId = priceListing.VendorId;
					existingPriceListing.DateListed = DateTime.Now;

					try
					{
						await priceListings.UpdateAsync(existingPriceListing);
					}
					catch (Exception ex)
					{
						ModelState.AddModelError("", $"Error: {ex.GetBaseException().Message}");
						return View(priceListing);
					}

				}
				await UpdateCheapestPrice(priceListing.ProductId, (Request.Form.ContainsKey("IsDiscounted") ? priceListing.DiscountedPrice : priceListing.Price));
				
				return RedirectToAction("ViewProduct", "Product", new { id = priceListing.ProductId });
			}

			return View(priceListing);
		}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var existingPriceListing = await priceListings.GetByIdAsync(id, new QueryOptions<PriceListing>());
            // Delete Product
            try
            {
                await priceListings.DeleteAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deleting listing: {ex.GetBaseException().Message}");
            }
            return RedirectToAction("ViewProduct", "Product", new { id = existingPriceListing.ProductId });
        }


		// Helper Methods -------------------------------------------------------
		public async Task UpdateCheapestPrice(int productId, decimal newPrice){
			Product exsitingProduct = await products.GetByIdAsync(productId, new QueryOptions<Product>());
			decimal oldPrice = exsitingProduct.CheapestPrice;
			if(newPrice < exsitingProduct.CheapestPrice || exsitingProduct.CheapestPrice == 0){ // Assume no listing has a price of 0
				exsitingProduct.CheapestPrice = newPrice;
				await products.UpdateAsync(exsitingProduct);

				// Send Notification to Users with item on wishlist if price drops
				await notificationService.CreateProductPriceDropNotifications(productId, exsitingProduct.Name, newPrice, oldPrice);
			}
		}

    }
}
