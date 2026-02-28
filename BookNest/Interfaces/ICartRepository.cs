using BookNest.DTO;
using BookNest.Models;
using Microsoft.EntityFrameworkCore;


namespace BookNest.Interfaces
{
    public interface ICartRepository
    {
        public Task AddToCart(Cart cartDTO);
        public Task<Book?> GetBookById(int id);
        public  Task UpdateBook(Book book);
        public Task<int> IncreaseQtyByOne(CartDTO cartDTO);
        public Task<int> DecreaseQtyByOne(CartDTO cartDTO);
        public Task<List<Cart>> GetCartLists();
        public Task<Cart?> GetCartByIdBookId(CartDTO cart);
        public Task UpdateCart(Cart cart);
        public Task<List<Cart>> GetCartListByUsername(string username);
        public Task<Cart?> GetCartByIdCartId(int cartId);
        public Task<int> GetCartListCount(string username);
        public Task DeleteCartList(List<Cart> cartItems);

    }
}
