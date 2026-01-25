using BookNest.DTO;

namespace BookNest.Interfaces
{
    public interface ICartRepository
    {
        public Task<int> AddToCart(CartDTO cartDTO);
        public Task<int> IncreaseQtyByOne(CartDTO cartDTO);
        public Task<int> DecreaseQtyByOne(CartDTO cartDTO);


    }
}
