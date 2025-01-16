using Microsoft.AspNetCore.Mvc;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Controllers
{
	public class PriceListingController : Controller
	{
		private Repository<PriceListing> priceListings;
		private Repository<Vendor> vendors;
		private Repository<Product> products;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public PriceListingController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			priceListings = new Repository<PriceListing>(context);
			vendors = new Repository<Vendor>(context);
			products = new Repository<Product>(context);
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
		public async Task<IActionResult> AddEdit(PriceListing priceListing, int vendorId)
		{
			ViewBag.Vendors = await vendors.GetAllAsync();
			if (ModelState.IsValid)
			{
                // Add Operation
                if (priceListing.PriceListingId == 0)
				{
					priceListing.DateListed = DateTime.Now;
					priceListing.VendorId = vendorId;
					await priceListings.AddAsync(priceListing);
				}
				else
				{
					var existingPriceListing = await priceListings.GetByIdAsync(priceListing.PriceListingId, new QueryOptions<PriceListing>());

                    if(existingPriceListing == null)
                    {
                        ModelState.AddModelError("", "Listing not found");
                        return View(priceListing);
                    }

					existingPriceListing.Price = priceListing.Price;
					existingPriceListing.PurchaseUrl = priceListing.PurchaseUrl;
					existingPriceListing.VendorId = vendorId;
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
			}
            return RedirectToAction("ViewProduct", "Product", new { id = priceListing.ProductId });
        }
    }
}
