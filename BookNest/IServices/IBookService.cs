using BookNest.DTO;
using BookNest.Models;

namespace BookNest.IServices
{
    public interface IBookService
    {
        public Task<List<Book>> GetYearBooks(string branch, string year);
        public Task<List<BookWithCartQtyDto>> GetYearBooks(string branch, string year, string username);
    }
}
