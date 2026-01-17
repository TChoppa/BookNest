using Microsoft.AspNetCore.Mvc;

namespace BookNest.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Books()
        {
            return View();
        }
        public IActionResult Journals()
        {
            return View();
        }
        public IActionResult ClubHost()
        {
            return View();
        }
    }
}
