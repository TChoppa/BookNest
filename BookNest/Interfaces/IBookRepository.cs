using BookNest.Models;

namespace BookNest.Interfaces
{
    public interface IBookRepository
    {
        public Task<List<Book>> GetYear1Books(string branch, string year);
    }
}
