using BookNest.DatabaseContext;
using BookNest.Interfaces;
using BookNest.IServices;
using Microsoft.AspNetCore.Mvc;

namespace BookNest.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        public BooksController(IBookService context)
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
        public IActionResult Year3()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Year4()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Journals()
        {
            return View();
        }
        [HttpPost]
        public  async Task<IActionResult> GetYearBooks(string branch , string year)
        {
            var username = HttpContext.Session.GetString("UserName");
            var books =  await _bookService.GetYearBooks(branch , year , username);
            return Ok(books);
        }
        [HttpGet]
        public IActionResult Year2()
        {
            return View();
        }

    }
}
