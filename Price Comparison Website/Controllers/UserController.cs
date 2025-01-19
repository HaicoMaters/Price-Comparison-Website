using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Price_Comparison_Website.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        public IActionResult Wishlist()
        {
            return View();
        }   
        public IActionResult ViewingHistory()
        {
            return View();
        }
    }
}
