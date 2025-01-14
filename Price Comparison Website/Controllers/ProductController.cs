
using Microsoft.AspNetCore.Mvc;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Controllers
{
    public class ProductController : Controller
    {
		private Repository<Product> products;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            products = new Repository<Product>(context);
            _webHostEnvironment = webHostEnvironment;
        }

		public async Task<IActionResult> Index()
        {
            return View(await products.GetAllAsync());
        }
    }
}
