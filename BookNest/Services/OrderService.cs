using BookNest.DatabaseContext;
using BookNest.Interfaces;
using BookNest.IServices;
using BookNest.Models;

namespace BookNest.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository)
        {
            _orderRepo = orderRepository;
            _cartRepo = cartRepository;
        }

        public async Task<string> ConfirmOrder(string username)
        {
            var cart = await _cartRepo.GetCartListByUsername(username);
            if (cart == null)
                return "false";
            var orderList = new Order
            {
                OrderCode = "ORD-" + DateTime.Now.ToString("yyyyMMdd") + "-" +
                Guid.NewGuid().ToString().Substring(0, 6),
                Username= username, 
                OrderDate = DateTime.Now,  
                Status  = "Pending"
            };
            await _orderRepo.AddOrder(orderList);
            foreach(var cartItem in cart)
            {
                var orderItemList = new OrderItem
                {
                    OrderId = orderList.OrderId,
                    BookId = cartItem.BookId,
                    ImageUrl = cartItem.ImageUrl,
                    Quantity = cartItem.AvailableQuantity,
                    Title=cartItem.Title
                };
                await _orderRepo.AddOrderItem(orderItemList);
            }
            await _cartRepo.DeleteCartList(cart);
            return orderList.OrderCode;
        }

        public async Task<List<Order>> GetOrderByUsername(string username)
        {
            return await _orderRepo.GetOrderByUsername(username);
        }
    }
}
