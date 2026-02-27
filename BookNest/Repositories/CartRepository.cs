using BookNest.DatabaseContext;
using BookNest.DTO;
using BookNest.Interfaces;
using BookNest.Models;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _dbContext;
        public CartRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Cart>> GetCartLists()
        {
            return await _dbContext.CartList.ToListAsync();
        }
        public async Task<List<Cart>> GetCartListByUsername(string username)
        {
            return await _dbContext.CartList.Where(x => x.Username == username && x.IsOrdered == false).ToListAsync();
        }
        public async Task<int> GetCartListCount(string username)
        {
            return await _dbContext.CartList.Where(x => x.Username == username && x.IsOrdered == false).SumAsync(x=>x.AvailableQuantity);
        }
        public async Task AddToCart(Cart cartDTO)
        {
            await _dbContext.CartList.AddAsync(cartDTO);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Book?> GetBookById(int id)
        {
            return await _dbContext.Books.Where(x => x.BookId == id).FirstOrDefaultAsync();
        }

        public async Task<Cart?> GetCartByIdBookId(CartDTO cart)
        {
            return await _dbContext.CartList.Where(x => x.BookId == cart.BookId && x.Username == cart.Username && x.IsOrdered == false).FirstOrDefaultAsync();
        }
        public async Task<Cart?> GetCartByIdCartId(int cartId)
        {
            return await _dbContext.CartList.Where(x=>x.CartId==cartId).FirstOrDefaultAsync();
        }
        public async Task UpdateCart(Cart cart)
        {
            _dbContext.CartList.Update(cart);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateBook(Book book)
        {
              _dbContext.Books.Update(book);
              await _dbContext.SaveChangesAsync();
        }   
        public async Task<int> IncreaseQtyByOne(CartDTO cartDTO)
        {
            int isQtyInc = 0;
            var cartItem = _dbContext.Books.Where(x=>x.BookId == cartDTO.BookId).FirstOrDefault();
            if(cartItem==null)
                return isQtyInc;
            cartItem.AvailableQuantity--;
            _dbContext.SaveChanges();
            isQtyInc = 1;
            return isQtyInc;
        }
        public async Task<int> DecreaseQtyByOne(CartDTO cartDTO)
        {
            int isQtyDec = 0;
            var cartItem = _dbContext.Books.Where(x => x.BookId == cartDTO.BookId).FirstOrDefault();
            if (cartItem == null)
                return isQtyDec;
            cartItem.AvailableQuantity++;
            _dbContext.SaveChanges();
            isQtyDec = 1;
            return isQtyDec;
        }
    }
}
