using BookNest.Interfaces;
using BookNest.Models;

namespace BookNest.Services
{
    public class BookService
    {
        private readonly IBookRepository _bookRepository;
        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public Task<List<Book>> GetYear1Books()
        {
            return _bookRepository.GetYear1Books();
        }
    }
}
