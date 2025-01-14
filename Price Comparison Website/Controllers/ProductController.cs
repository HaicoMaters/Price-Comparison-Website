using Microsoft.AspNetCore.Mvc;

namespace Price_Comparison_Website.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
