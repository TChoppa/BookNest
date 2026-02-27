using BookNest.DTO;
using BookNest.Models;

namespace BookNest.IServices
{
    public interface IBookService
    {
        public Task<List<Book>> GetYear1Books(string branch, string year);
        public Task<List<BookWithCartQtyDto>> GetYear1Books(string branch, string year, string username);
    }
}
