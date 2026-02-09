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

        public async Task<List<Book>> GetYear1Books(string branch, string year)
        {
            return _dbContext.Books
                .Where(b => b.Year == year || b.BranchCode==branch).ToList();           
        }
    }
}
