using BookNest.DTO;
using BookNest.Enums;
using BookNest.IServices;
using BookNest.Models;
using BookNest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BookNest.Controllers
{
    public class CartControllercs : Controller
    {
        private readonly ICartService _cartService; 
        public CartControllercs(ICartService cartService)
        {
            _cartService= cartService;
        }
        public async Task<IActionResult> Index()
        {
            var cartList = await _cartService.GetCartList();
            return View(cartList);
        }
        
        public async Task<IActionResult> ViewCart()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var cartList = await _cartService.GetCartListByUsername(userName);
            return View("Index",cartList);
        }
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var cartList = await _cartService.GetCartListCount(userName);
            return Ok(cartList);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartDTO cart)
        {
            if (cart == null)
                return BadRequest(new { success = false , message="Invalid CartList"});
            var result = await _cartService.AddToCart(cart);
            var res = result switch
            {
                AddToCartEnum.BookNotFound => new { success = false, message = "Book Not Found" },
                AddToCartEnum.BookOutOfStock => new { success = false, message = "Book Out Of Stock" },
                AddToCartEnum.Success => new { success = true, message = "Added Sucessfully" },
                _=> new {success= false , message=""}
                
            };
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQtyByOne([FromBody] CartDTO cartDto)
        {
            if (cartDto == null)
                return BadRequest(new { success = false, message = "" });
            var result = await _cartService.IncreaseQtyByOne(cartDto);
            var res = result switch
            {
                AddToCartEnum.BookNotFound => new { success = false, message = "Book Not Found" },
                AddToCartEnum.BookOutOfStock => new { success = false, message = "Book Out Of Stock" },
                AddToCartEnum.Success => new { success = true, message = "Added Sucessfully" },
                _ => new { success = false, message = "" }

            };
            return Ok(res);
        }
        [HttpPost]
        public async Task<IActionResult> DecreaseQtyByOne([FromBody] CartDTO cartDto)
        {
            if (cartDto == null)
                return Unauthorized(new { success = false, message = "" });
            var result = await _cartService.DecreaseQtyByOne(cartDto);
            var res = result switch
            {
                AddToCartEnum.BookNotFound => new { success = false, message = "Book Not Found" },
                AddToCartEnum.BookOutOfStock => new { success = false, message = "Book Out Of Stock" },
                AddToCartEnum.CartNotFound => new { success = false, message = "Cart Not Found" },
                AddToCartEnum.Success => new { success = true, message = "Added Sucessfully" },
                _ => new { success = false, message = "" }

            };
            return Ok(res);
        }
        [HttpPost]
        public async Task<IActionResult> IncreaseQtyByOneInCart(int cartId)
        {
            if (cartId==0)
                return BadRequest(new { success = false, message = "" });
            var result = await _cartService.IncreaseQtyByOneInCart(cartId);
            var res = result switch
            {
                AddToCartEnum.BookNotFound => new { success = false, message = "Book Not Found" },
                AddToCartEnum.BookOutOfStock => new { success = false, message = "Book Out Of Stock" },
                AddToCartEnum.CartNotFound => new { success = false, message = "Cart Not Found" },
                AddToCartEnum.Success => new { success = true, message = "Added Sucessfully" },
                _ => new { success = false, message = "" }

            };
            return Ok(res);
        }
        [HttpPost]
        public async Task<IActionResult> DecreaseQtyByOneInCart(int cartId)
        {
            if (cartId == 0)
                return BadRequest(new { success = false, message = "" });
            var result = await _cartService.DecreaseQtyByOneInCart(cartId);
            var res = result switch
            {
                AddToCartEnum.BookNotFound => new { success = false, message = "Book Not Found" },
                AddToCartEnum.BookOutOfStock => new { success = false, message = "Book Out Of Stock" },
                AddToCartEnum.CartNotFound => new { success = false, message = "Cart Not Found" },
                AddToCartEnum.Success => new { success = true, message = "Added Sucessfully" },
                _ => new { success = false, message = "" }

            };
            return Ok(res);
        }


    }
}
