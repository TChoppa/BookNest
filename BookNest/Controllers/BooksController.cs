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
        [HttpPost]
        public  async Task<IActionResult> GetYear1Books(string branch , string year)
        {
            var username = HttpContext.Session.GetString("UserName");
            var books =  await _bookService.GetYear1Books(branch , year , username);
            return Ok(books);
        }

    }
}
