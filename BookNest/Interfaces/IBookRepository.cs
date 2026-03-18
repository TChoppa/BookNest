using BookNest.Models;

namespace BookNest.Interfaces
{
    public interface IBookRepository
    {
        public Task<List<Book>> GetYearBooks(string branch, string year);
        public Task<Cart?> GetBookById(int bookId, string userName);
        public Task<Book> GetBookByIds(int bookId);
        public Task<List<Book>> GetMultipleBookByIds(IEnumerable<int> bookIds);
    }
}
