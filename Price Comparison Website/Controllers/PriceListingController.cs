using Microsoft.AspNetCore.Mvc;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Controllers
{
	public class PriceListingController : Controller
	{
		private Repository<PriceListing> priceListings;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public PriceListingController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			priceListings = new Repository<PriceListing>(context);
			_webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public async Task<IActionResult> AddEdit(int id)
		{

			return View();
		}
	}
}
