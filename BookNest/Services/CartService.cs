using BookNest.DTO;
using BookNest.Enums;
using BookNest.Interfaces;
using BookNest.IServices;
using BookNest.Migrations;
using BookNest.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Cart = BookNest.Models.Cart;

namespace BookNest.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepo;
        public CartService(ICartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }
        public async Task<List<Cart>> GetCartList()
        {
            return await _cartRepo.GetCartLists();
        }
        public async Task<List<Cart>> GetCartListByUsername(string username)
        {
            return await _cartRepo.GetCartListByUsername(username);
        }
        public async Task<int> GetCartListCount(string username)
        {
            return await _cartRepo.GetCartListCount(username);
        }
        public async Task<AddToCartEnum> AddToCart(CartDTO cart)
        {
            var existingCart = await _cartRepo.GetCartByIdBookId(cart);
            //var book = await _cartRepo.GetBookById(cart.BookId);
            //if (book == null)
            //    return AddToCartEnum.BookNotFound;
            //if (book.AvailableQuantity <= 0)
            //    return AddToCartEnum.BookOutOfStock;
            if (existingCart != null)
            {
                existingCart.AvailableQuantity += 1;
                await _cartRepo.UpdateCart(existingCart);
              //  book.AvailableQuantity--;
               // await _cartRepo.UpdateBook(book);
                return AddToCartEnum.Success;
            } 
            var carts = new Cart
            {
                Username = cart.Username,
                BookId = cart.BookId,
                ImageUrl = cart.ImageUrl,
                Title= cart.Title,
                Year= cart.Year,
                AvailableQuantity = 1,
                IsOrdered = false
            };
            await _cartRepo.AddToCart(carts);
            //book.AvailableQuantity--;
           // await _cartRepo.UpdateBook(book);
            return AddToCartEnum.Success;
        }
        public async Task<AddToCartEnum> IncreaseQtyByOne(CartDTO cart)
        {
            //var book = await _cartRepo.GetBookById(cart.BookId);
            //if (book == null)
            //    return AddToCartEnum.BookNotFound;
            //if (book.AvailableQuantity <= 0)
            //    return AddToCartEnum.BookOutOfStock;
            var existingCart = await _cartRepo.GetCartByIdBookId(cart);                 
            if (existingCart == null)
                return AddToCartEnum.BookNotFound;
            existingCart.AvailableQuantity += 1;
           // book.AvailableQuantity--;
            await _cartRepo.UpdateCart(existingCart);
           // await _cartRepo.UpdateBook(book);
            return AddToCartEnum.Success;
        }
        public async Task<AddToCartEnum> DecreaseQtyByOne(CartDTO cart)
        {
            //var book = await _cartRepo.GetBookById(cart.BookId);
            //if (book == null)
            //    return AddToCartEnum.BookNotFound;
            var existingCart = await _cartRepo.GetCartByIdBookId(cart);          
            if (existingCart == null)
                return AddToCartEnum.CartNotFound;           
            //if (book.AvailableQuantity <= 0)
            //    return AddToCartEnum.BookOutOfStock;                  
            existingCart.AvailableQuantity -= 1;          
            //book.AvailableQuantity++;
            await _cartRepo.UpdateCart(existingCart);
          //  await _cartRepo.UpdateBook(book);
            return AddToCartEnum.Success;
        }

        public async Task<AddToCartEnum> IncreaseQtyByOneInCart(int cartId)
        {
            var cartItem = await _cartRepo.GetCartByIdCartId(cartId);
            if(cartItem == null)
                return AddToCartEnum.CartNotFound;
            cartItem.AvailableQuantity++;
            await _cartRepo.UpdateCart(cartItem);
            //var book = await _cartRepo.GetBookById(cartItem.BookId);
            //if(book==null)
            //    return AddToCartEnum.BookNotFound;
            //book.AvailableQuantity--;
            //await _cartRepo.UpdateBook(book);
            return AddToCartEnum.Success;
        }
        public async Task<AddToCartEnum> DecreaseQtyByOneInCart(int cartId)
        {
            var cartItem = await _cartRepo.GetCartByIdCartId(cartId);
            if (cartItem == null)
                return AddToCartEnum.CartNotFound;
            //var book = await _cartRepo.GetBookById(cartItem.BookId);
            //if (book == null)
            //    return AddToCartEnum.BookNotFound;           
            cartItem.AvailableQuantity--;                     
            //book.AvailableQuantity++;
            await _cartRepo.UpdateCart(cartItem);
           // await _cartRepo.UpdateBook(book);
            return AddToCartEnum.Success;
        }
    }
}
