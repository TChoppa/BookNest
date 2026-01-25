using BookNest.DatabaseContext;
using BookNest.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookNest.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookService;
        public BooksController(IBookRepository context)
        {
            _bookService = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Year1()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetYear1Books()
        {
            var books = _bookService.GetYear1Books();

            return Json(books);
        }

    }
}
