using BookNest.DatabaseContext;
using BookNest.DTO;
using BookNest.Interfaces;
using BookNest.Models;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;
        public BookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Book>> GetYearBooks(string branch, string year)
        {
            return _dbContext.Books
                .Where(b => b.Year == year && b.BranchCode==branch).ToList();           
        }
        public async Task<Cart?> GetBookById(int bookId, string userName)
        {
            return await _dbContext.CartList.Where(x => x.BookId == bookId && x.Username == userName && !x.IsOrdered).FirstOrDefaultAsync();
        }
      
        public async Task<Book> GetBookByIds(int bookId)
        {
            return await _dbContext.Books.Where(x => x.BookId == bookId ).FirstOrDefaultAsync();
        }
        public async Task<List<Book>> GetMultipleBookByIds(IEnumerable<int> bookIds)
        {
            // Example using Entity Framework
            return await _dbContext.Books
                                 .Where(b => bookIds.Contains(b.BookId))
                                 .ToListAsync();
        }

    }
}
