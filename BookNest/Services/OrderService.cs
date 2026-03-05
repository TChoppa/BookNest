using BookNest.DatabaseContext;
using BookNest.DTO;
using BookNest.Interfaces;
using BookNest.IServices;
using BookNest.Models;

namespace BookNest.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IBookRepository _bookRepo;
        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository , IBookRepository bookRepository)
        {
            _orderRepo = orderRepository;
            _cartRepo = cartRepository;
            _bookRepo = bookRepository;
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
                Status  = "Pending",
                ReturnDate = DateTime.Now.AddDays(7)
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
                    Title = cartItem.Title
                    
                };
                await _orderRepo.AddOrderItem(orderItemList);
                 var book = await _bookRepo.GetBookByIds(cartItem.BookId);

                if (book != null)
                {
                    book.AvailableQuantity -= cartItem.AvailableQuantity;

                    if (book.AvailableQuantity < 0)
                        book.AvailableQuantity = 0; // safety

                    await _cartRepo.UpdateBook(book);
                }
            }
            await _cartRepo.DeleteCartList(cart);
            return orderList.OrderCode;
        }

        public async Task<List<Order>> GetOrderByUsername(string username)
        {
            return await _orderRepo.GetOrderByUsername(username);
        }
        public async Task<List<OrderIItemDTO>> GetOrderItemsByUsername(string username)
        {
            var orders = await _orderRepo.GetOrderByUsername(username);
            var orderItemDtos = new List<OrderIItemDTO>();
            if (orders == null || !orders.Any())
                return orderItemDtos;

            foreach (var order in orders)
            {
                var orderItems = await _orderRepo.GetOrderItemByUsername(order.OrderId);
                if (orderItems == null || !orderItems.Any())
                    continue;

                foreach (var orderItem in orderItems)
                {
                    var orderItemDto = new OrderIItemDTO
                    {
                        OrderCode = order.OrderCode,
                        ImageUrl = orderItem.ImageUrl ?? string.Empty,
                        Title = orderItem.Title,
                        OrderDate = order.OrderDate,
                        Quantity = orderItem.Quantity,
                        ReturnDate = order.ReturnDate,
                        FineAmount = order.FineAmount,
                        Status=order.Status
                    };
                    orderItemDtos.Add(orderItemDto);
                }
            }

            return orderItemDtos;
        }

    }
}
