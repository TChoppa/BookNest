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

        public async Task<int> AddToCart(CartDTO cartDTO)
        {
            int isItemAdded = 0;
            var cartItem = new Cart()
            {
                Username = cartDTO.Username,
                BookId = cartDTO.BookId,
                Title = cartDTO.Title,
                ImageUrl = cartDTO.ImageUrl,
                AvailableQuantity=cartDTO.AvailableQuantity,
                Year=cartDTO.Year
            };
             _dbContext.Add(cartItem);
            await _dbContext.SaveChangesAsync();
            isItemAdded = 1;
            return isItemAdded;
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
