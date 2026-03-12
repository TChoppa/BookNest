using System;
using System.Linq;
using BookNest.DatabaseContext;
using BookNest.DTO;
using BookNest.Interfaces;
using BookNest.IServices;
using BookNest.Models;
using Microsoft.AspNetCore.Identity;

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
                if (cartItem.AvailableQuantity == 0)
                {
                    break; 
                }
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

        //public async Task<(List<OrderIItemDTO> orders, int totalCount)> GetAllOrders(int page, int pageSize, string? search)
        //{
        //    var allOrders = await _orderRepo.GetAllOrders();
        //    if (allOrders == null)
        //        allOrders = new List<Order>();

        //    // filter
        //    if (!string.IsNullOrWhiteSpace(search))
        //    {
        //        search = search.Trim();
        //        allOrders = allOrders.Where(o => (o.OrderCode ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase)
        //                                  || (o.Username ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase))
        //                               .ToList();
        //    }

        //    var total = allOrders.Count;

        //    var paged = allOrders.OrderByDescending(o => o.OrderDate)
        //                         .Skip((Math.Max(1, page) - 1) * pageSize)
        //                         .Take(pageSize)
        //                         .ToList();

        //    var orderItemDtos = new List<OrderIItemDTO>();
        //    foreach (var order in paged)
        //    {
        //        var orderItems = await _orderRepo.GetOrderItemByUsername(order.OrderId);
        //        var totalQty = orderItems?.Sum(i => i.Quantity) ?? 0;
        //        var firstImage = orderItems?.FirstOrDefault()?.ImageUrl ?? string.Empty;

        //        var dto = new OrderIItemDTO
        //        {
        //            OrderCode = order.OrderCode,
        //            ImageUrl = firstImage,
        //            Title = order.Username, // show username for admin listing
        //            OrderDate = order.OrderDate,
        //            Quantity = totalQty,
        //            ReturnDate = order.ReturnDate,
        //            FineAmount = order.FineAmount,
        //            Status = order.Status
        //        };
        //        orderItemDtos.Add(dto);
        //    }

        //    return (orderItemDtos, total);
        //}
        public async Task<(List<OrderIItemDTO> orderItems, int totalCount)> GetAllOrders(int page, int pageSize, string? search)
        {
            var allOrders = await _orderRepo.GetAllOrders() ?? new List<Order>();

            // build lookup for quick access
            var orderLookup = allOrders.ToDictionary(o => o.OrderId);

            // flatten all order items
            var allOrderItems = new List<OrderItem>();
            foreach (var order in allOrders)
            {
                var items = await _orderRepo.GetOrderItemByUsername(order.OrderId);
                if (items != null)
                    allOrderItems.AddRange(items);
            }

            // filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                allOrderItems = allOrderItems.Where(i =>
                    (i.Title ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase)
                    || (orderLookup[i.OrderId].OrderCode ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase)
                    || (orderLookup[i.OrderId].Username ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var total = allOrderItems.Count;

            // pagination on items
            var pagedItems = allOrderItems
                .OrderByDescending(i => orderLookup[i.OrderId].OrderDate)
                .Skip((Math.Max(1, page) - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // map to DTO
            var itemDtos = pagedItems.Select(i =>
            {
                var parent = orderLookup[i.OrderId];
                int fineAmount = 0;
                if (parent.ReturnDate.HasValue)
                {
                    var totalDays = (parent.ReturnDate.Value.Date - parent.OrderDate.Date).Days;
                    if (totalDays > 7)
                        fineAmount = (totalDays - 7) * 5; // ₹5 per extra day
                }
                return new OrderIItemDTO
                {
                    OrderItemId = i.OrderItemId,
                    OrderId = parent.OrderId,
                    OrderCode = parent.OrderCode,          
                    Title = i.Title,
                    ImageUrl = i.ImageUrl,
                    Quantity = i.Quantity,
                    OrderDate = parent.OrderDate,
                    Status = parent.Status,
                    ReturnDate = parent.ReturnDate,
                    FineAmount = fineAmount,
                    Action = parent.Action,
                    ReturnStatus = parent.ReturnStatus,
                    Username=parent.Username

                };
            }).ToList();

            return (itemDtos, total);
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
                        OrderItemId = orderItem.OrderItemId,
                        OrderId = order.OrderId,
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

        public async Task<bool> DeleteOrderItemForUser(string username, int orderItemId)
        {
            var item = await _orderRepo.GetOrderItemById(orderItemId);
            if (item == null)
                return false;

            var parent = await _orderRepo.GetOrderById(item.OrderId);
            if (parent == null)
                return false;

            // ensure the item belongs to the requesting user
            if (!string.Equals(parent.Username, username, StringComparison.OrdinalIgnoreCase))
                return false;

            await _orderRepo.DeleteOrderItem(orderItemId);
            return true;
        }

        // paged per-user order items with optional search
        public async Task<(List<OrderIItemDTO> orders, int totalCount)> GetOrderItemsByUsername(string username, int page, int pageSize, string? search)
        {
            var orders = await _orderRepo.GetOrderByUsername(username) ?? new List<Order>();
            if (!orders.Any())
                return (new List<OrderIItemDTO>(), 0);

            // build lookup
            var orderLookup = orders.ToDictionary(o => o.OrderId);

            var allItems = new List<OrderItem>();
            foreach (var order in orders)
            {
                var items = await _orderRepo.GetOrderItemByUsername(order.OrderId);
                if (items != null)
                    allItems.AddRange(items.Select(i => {
                        // attach order id already present
                        return i;
                    }));
            }

            // filter by search on title or ordercode
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                allItems = allItems.Where(i => (i.Title ?? string.Empty).Contains(s, StringComparison.OrdinalIgnoreCase)
                                              || (orderLookup[i.OrderId].OrderCode ?? string.Empty).Contains(s, StringComparison.OrdinalIgnoreCase))
                                    .ToList();
            }

            var total = allItems.Count;

            var paged = allItems.OrderByDescending(i => orderLookup[i.OrderId].OrderDate)
                                .Skip((Math.Max(1, page) - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            var dtos = paged.Select(i =>
            {
                var parent = orderLookup[i.OrderId];
                int fineAmount = 0;
                if (parent.ReturnDate.HasValue)
                {
                    var totalDays = (parent.ReturnDate.Value.Date - parent.OrderDate.Date).Days;
                    if (totalDays > 7)
                        fineAmount = (totalDays - 7) * 5; // ₹5 per extra day
                }
                return new OrderIItemDTO
                {
                    OrderId=parent.OrderId,
                    OrderItemId=i.OrderItemId,
                    OrderCode = parent.OrderCode,
                    ImageUrl = i.ImageUrl ?? string.Empty,
                    Title = i.Title,
                    OrderDate = parent.OrderDate,
                    Quantity = i.Quantity,
                    ReturnDate = parent.ReturnDate,
                    FineAmount = fineAmount,
                    Status = parent.Status
                };
            }).ToList();

            return (dtos, total);
        }

        public async Task<UpdateOrderDTO> UpdateOrderStatus(int orderItemId)
        {
            var orderItem = await _orderRepo.GetOrderItemById(orderItemId);
            if (orderItem == null) return null;
            var order = await _orderRepo.GetOrderById(orderItem.OrderId);
            if (order == null) return null;
            order.Status = "Approved";
            order.ReturnStatus = "Pending";
            order.Action = true;
            var res = await _orderRepo.UpdateOrder(order);
            if(res == null) return null;
            
            var updateOrder = new UpdateOrderDTO
            {
                FineAmount = res.FineAmount,
                Action = res.Action,
                ReturnStatus = res.ReturnStatus
            };
            return updateOrder;
        }
        public async Task<UpdateOrderDTO> UpdateReturnOrderStatus(int orderItemId)
        {
            var orderItem = await _orderRepo.GetOrderItemById(orderItemId);
            if (orderItem == null) return null;
            var order = await _orderRepo.GetOrderById(orderItem.OrderId);
            if (order == null) return null;
            order.Status = "Cleared";
            order.ReturnStatus = "Returned";
            order.Action = true;
            var res = await _orderRepo.UpdateOrder(order);
            if (res == null) return null;
            var book = await _bookRepo.GetBookByIds(orderItem.BookId);

            if (book != null)
            {
                book.AvailableQuantity += orderItem.Quantity;

                if (book.AvailableQuantity < 0)
                    book.AvailableQuantity = 0; // safety

                await _cartRepo.UpdateBook(book);
            }
            var updateOrder = new UpdateOrderDTO
            {
                FineAmount = res.FineAmount,
                Action = res.Action,
                ReturnStatus = res.ReturnStatus
            };
            return updateOrder;
        }
    }
}
