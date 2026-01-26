using BookNest.DTO;
using BookNest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BookNest.Controllers
{
    public class CartControllercs : Controller
    {
        private readonly CartService _cartService; 
        public CartControllercs(CartService cartService)
        {
            _cartService= cartService;
        }
        public async Task<IActionResult> Index()
        {
            var cartList = await _cartService.GetCartList();
            return View(cartList);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartDTO cartDto)
        {
            if (cartDto == null)
                return Unauthorized(new { success = false , message=""});
            var isSuccess = await _cartService.AddToCart(cartDto);
            if (isSuccess == 1)
                return Json(new { success = true, message = "" });
            return Json(new { success = false, message = "Invalid Credentials " });
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQtyByOne([FromBody] CartDTO cartDto)
        {
            if (cartDto == null)
                return Unauthorized(new { success = false, message = "" });
            var isSuccess = await _cartService.IncreaseQtyByOne(cartDto);
            if (isSuccess == 1)
                return Json(new { success = true, message = "" });
            return Json(new { success = false, message = "Invalid Credentials " });
        }
        [HttpPost]
        public async Task<IActionResult> DecreaseQtyByOne([FromBody] CartDTO cartDto)
        {
            if (cartDto == null)
                return Unauthorized(new { success = false, message = "" });
            var isSuccess = await _cartService.DecreaseQtyByOne(cartDto);
            if (isSuccess == 1)
                return Json(new { success = true, message = "" });
            return Json(new { success = false, message = "Invalid Credentials " });
        }
    }
}
