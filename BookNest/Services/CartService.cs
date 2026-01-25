using BookNest.DTO;
using BookNest.Interfaces;

namespace BookNest.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepo;
        public CartService(ICartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }

        public Task<int> AddToCart(CartDTO cartDTO)
        {
            return _cartRepo.AddToCart(cartDTO);
        }
        public Task<int> IncreaseQtyByOne(CartDTO cartDTO)
        {
            return _cartRepo.IncreaseQtyByOne(cartDTO);
        }
        public Task<int> DecreaseQtyByOne(CartDTO cartDTO)
        {
            return _cartRepo.DecreaseQtyByOne(cartDTO);
        }
    }
}
