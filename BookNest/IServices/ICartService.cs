using BookNest.DTO;
using BookNest.Enums;
using Cart = BookNest.Models.Cart;

namespace BookNest.IServices
{
    public interface ICartService
    {
        public  Task<List<Cart>> GetCartList();
        public Task<List<Cart>> GetCartListByUsername(string username);
        public Task<AddToCartEnum> AddToCart(CartDTO cart);
        public Task<AddToCartEnum> IncreaseQtyByOne(CartDTO cartDTO);

        public Task<AddToCartEnum> DecreaseQtyByOne(CartDTO cartDTO);
        public Task<AddToCartEnum> IncreaseQtyByOneInCart(int cartId);
        public Task<AddToCartEnum> DecreaseQtyByOneInCart(int cartId);
        public Task<int> GetCartListCount(string username);
    }
}
