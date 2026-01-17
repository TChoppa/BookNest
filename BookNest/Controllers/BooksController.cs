using Microsoft.AspNetCore.Mvc;

namespace BookNest.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Year1()
        {
            ViewBag.PageTitle = "Books";
            ViewBag.CurrentSubPage = "Year1";
            return View();
        }

    }
}
