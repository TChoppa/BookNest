using BookNest.DTO;
using BookNest.Interfaces;
using BookNest.IServices;
using BookNest.Models;
using BookNest.Repositories;
using System.Net;

namespace BookNest.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRep;
        private readonly ICartRepository _carRepo;
        public BookService(IBookRepository bookRepository , ICartRepository cartRepository)
        {
            _bookRep = bookRepository;
            _carRepo = cartRepository;
        }
        public async Task<List<Book>> GetYear1Books(string branch, string year)
        {
            return await _bookRep.GetYear1Books(branch, year);
        }


        public async Task<List<BookWithCartQtyDto>> GetYear1Books(string branch, string year, string username)
        {
            // 1️⃣ Get books
            var books = await _bookRep.GetYear1Books(branch, year);
            if (books == null || !books.Any())
                return new List<BookWithCartQtyDto>();

            // 2️⃣ Get cart items for user
            var cartItems = await _carRepo.GetCartListByUsername(username);

            // 3️⃣ Merge book + cart qty
            var result = books.Select(b => new BookWithCartQtyDto
            {
                BookId = b.BookId,
                Title = b.Title,
                ImageUrl = b.ImageUrl,
                AvailableQuantity = b.AvailableQuantity,
                Year = b.Year,

                // 🔑 get qty from CartList table
                CartQty = cartItems
           .Where(c => c.BookId == b.BookId && !c.IsOrdered)
           .Select(c => c.AvailableQuantity)
           .FirstOrDefault()
            }).ToList();

            return result;
        }



    }
}
