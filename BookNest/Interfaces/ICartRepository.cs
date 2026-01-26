using BookNest.DTO;
using BookNest.Models;


namespace BookNest.Interfaces
{
    public interface ICartRepository
    {
        public Task<int> AddToCart(CartDTO cartDTO);
        public Task<int> IncreaseQtyByOne(CartDTO cartDTO);
        public Task<int> DecreaseQtyByOne(CartDTO cartDTO);
        public Task<List<Cart>> GetCartLists();

    }
}
