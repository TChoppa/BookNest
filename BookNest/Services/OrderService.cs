using BookNest.DatabaseContext;
using BookNest.DTO;
using BookNest.Interfaces;
using BookNest.IServices;
using BookNest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Linq;

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
                    Title = cartItem.Title,
                    Status= "Pending"

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

            var bookIds = pagedItems.Select(i => i.BookId).ToList();
            var books = await _bookRepo.GetMultipleBookByIds(bookIds);
            var bookLookup = books.ToDictionary(b => b.BookId);

            // map to DTO
            var itemDtos = pagedItems.Select(i =>
            {
                var parent = orderLookup[i.OrderId];
                int fineAmount = 0;

                if (parent.ReturnDate.HasValue)
                {
                    if (i.ReturnStatus != "Returned")
                    {
                        // Still not returned → fine keeps accumulating until today
                        var overdueDays = (DateTime.UtcNow.Date - parent.ReturnDate.Value.Date).Days;
                        if (overdueDays > 0)
                            fineAmount = overdueDays * 5;
                    }
                    else
                    {
                        // Returned → fine stops at the return date
                        var overdueDays = (parent.ReturnDate.Value.Date - parent.OrderDate.Date).Days - 7;
                        if (overdueDays > 0)
                            fineAmount = overdueDays * 5;
                    }
                }

                var book = bookLookup.TryGetValue(i.BookId, out var b) ? b : null;
                return new OrderIItemDTO
                {
                    OrderItemId = i.OrderItemId,
                    OrderId = parent.OrderId,
                    OrderCode = parent.OrderCode,
                    Title = i.Title,
                    ImageUrl = i.ImageUrl,
                    Quantity = i.Quantity,
                    OrderDate = parent.OrderDate,
                    Status = i.Status,
                    ReturnDate = parent.ReturnDate,
                    FineAmount = fineAmount,
                    Action = i.Action,
                    ReturnStatus = i.ReturnStatus,
                    Username = parent.Username,
                    Year = book?.Year
                };
            }).ToList();

            return (itemDtos, total);
        }

        public async Task<(List<OrderIItemDTO> data, int totalCount)>
    GetOverdueNotClearedOrders(int page, int pageSize, string? search)
        {
            var (rawData, totalCount) = await _orderRepo
                .GetOverdueNotClearedOrders(page, pageSize, search);

            var bookIds = rawData.Select(x => x.oi.BookId).Distinct().ToList();
            var books = await _bookRepo.GetMultipleBookByIds(bookIds);
            var bookLookup = books.ToDictionary(b => b.BookId);

            var result = rawData.Select(x =>
            {
                var oi = x.oi;
                var o = x.o;

                int fineAmount = 0;

                if (o.ReturnDate.HasValue)
                {
                    if (oi.ReturnStatus != "Returned")
                    {
                        var overdueDays = (DateTime.UtcNow.Date - o.ReturnDate.Value.Date).Days;
                        if (overdueDays > 0)
                            fineAmount = overdueDays * 5;
                    }
                    else
                    {
                        var overdueDays = (o.ReturnDate.Value.Date - o.OrderDate.Date).Days - 7;
                        if (overdueDays > 0)
                            fineAmount = overdueDays * 5;
                    }
                }

                var book = bookLookup.TryGetValue(oi.BookId, out var b) ? b : null;

                return new OrderIItemDTO
                {
                    OrderItemId = oi.OrderItemId,
                    OrderId = o.OrderId,
                    OrderCode = o.OrderCode,
                    Title = oi.Title,
                    ImageUrl = oi.ImageUrl,
                    Quantity = oi.Quantity,
                    OrderDate = o.OrderDate,
                    Status = oi.Status,
                    ReturnDate = o.ReturnDate,
                    FineAmount = fineAmount,
                    Action = oi.Action,
                    ReturnStatus = oi.ReturnStatus,
                    Username = o.Username,
                    Year = book?.Year
                };
            }).ToList();

            return (result, totalCount);
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
                        FineAmount = orderItem.FineAmount,
                        Status=orderItem.Status
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

        public async Task<(List<OrderIItemDTO> orders, int totalCount)> GetOrderItemsByUsername(
       string username, int page, int pageSize, string? search)
        {
            var orders = await _orderRepo.GetOrderByUsername(username) ?? new List<Order>();
            if (!orders.Any())
                return (new List<OrderIItemDTO>(), 0);

            // Build lookup for orders
            var orderLookup = orders.ToDictionary(o => o.OrderId);

            // Collect all items across orders
            var allItems = new List<OrderItem>();
            foreach (var order in orders)
            {
                var items = await _orderRepo.GetOrderItemByUsername(order.OrderId);
                if (items != null)
                    allItems.AddRange(items);
            }

            // Filter by search on title or order code
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                allItems = allItems.Where(i =>
                    (i.Title ?? string.Empty).Contains(s, StringComparison.OrdinalIgnoreCase) ||
                    (orderLookup[i.OrderId].OrderCode ?? string.Empty).Contains(s, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var total = allItems.Count;

            // Apply paging
            var paged = allItems.OrderByDescending(i => orderLookup[i.OrderId].OrderDate)
                                .Skip((Math.Max(1, page) - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            // Fetch all books for the paged items in one go
            var bookIds = paged.Select(i => i.BookId).ToList();
            var books = await _bookRepo.GetMultipleBookByIds(bookIds);
            var bookLookup = books.ToDictionary(b => b.BookId);

            // Map to DTOs sequentially
            var dtos = new List<OrderIItemDTO>();
            foreach (var i in paged)
            {
                var parent = orderLookup[i.OrderId];

                int fineAmount = 0;
                if (parent.ReturnDate.HasValue)
                {
                    if (i.ReturnStatus != "Returned")
                    {
                        // Not returned yet → fine keeps accumulating until today
                        var overdueDays = (DateTime.UtcNow.Date - parent.ReturnDate.Value.Date).Days;
                        if (overdueDays > 0)
                            fineAmount = overdueDays * 5;
                    }
                    else
                    {
                        // Returned → fine stops at the return date
                        var overdueDays = (parent.ReturnDate.Value.Date - parent.OrderDate.Date).Days - 7;
                        if (overdueDays > 0)
                            fineAmount = overdueDays * 5;
                    }
                    i.FineAmount = fineAmount;
                }

                // Safe sequential update
                await _orderRepo.UpdateOrderItem(i);

                var book = bookLookup.TryGetValue(i.BookId, out var b) ? b : null;

                dtos.Add(new OrderIItemDTO
                {
                    OrderId = parent.OrderId,
                    OrderItemId = i.OrderItemId,
                    OrderCode = parent.OrderCode,
                    ImageUrl = i.ImageUrl ?? string.Empty,
                    Title = i.Title,
                    OrderDate = parent.OrderDate,
                    Quantity = i.Quantity,
                    ReturnDate = parent.ReturnDate,
                    FineAmount = fineAmount,
                    Status = i.Status,
                    Year = book?.Year
                });
            }

            return (dtos, total);
        }

        public async Task<List<OrderItem>>GetAllOrderItems()
        {
            return await _orderRepo.GetAllOrderItems();
        }

        //    public async Task<(List<OrderIItemDTO> orders, int totalCount)> GetOrderItemsByUsername(
        //string username, int page, int pageSize, string? search)
        //    {
        //        var orders = await _orderRepo.GetOrderByUsername(username) ?? new List<Order>();
        //        if (!orders.Any())
        //            return (new List<OrderIItemDTO>(), 0);

        //        // Build lookup for orders
        //        var orderLookup = orders.ToDictionary(o => o.OrderId);

        //        // Collect all items across orders
        //        var allItems = new List<OrderItem>();
        //        foreach (var order in orders)
        //        {
        //            var items = await _orderRepo.GetOrderItemByUsername(order.OrderId);
        //            if (items != null)
        //                allItems.AddRange(items);
        //        }

        //        // Filter by search on title or order code
        //        if (!string.IsNullOrWhiteSpace(search))
        //        {
        //            var s = search.Trim();
        //            allItems = allItems.Where(i =>
        //                (i.Title ?? string.Empty).Contains(s, StringComparison.OrdinalIgnoreCase) ||
        //                (orderLookup[i.OrderId].OrderCode ?? string.Empty).Contains(s, StringComparison.OrdinalIgnoreCase))
        //                .ToList();
        //        }

        //        var total = allItems.Count;

        //        // Apply paging
        //        var paged = allItems.OrderByDescending(i => orderLookup[i.OrderId].OrderDate)
        //                            .Skip((Math.Max(1, page) - 1) * pageSize)
        //                            .Take(pageSize)
        //                            .ToList();

        //        // Fetch all books for the paged items in one go
        //        var bookIds = paged.Select(i => i.BookId).ToList();
        //        var books = await _bookRepo.GetMultipleBookByIds(bookIds);
        //        var bookLookup = books.ToDictionary(b => b.BookId);

        //        // Map to DTOs
        //        var dtos = paged.Select(async i =>
        //        {
        //            var parent = orderLookup[i.OrderId];
        //            int fineAmount = 0;
        //            if (parent.ReturnDate.HasValue)
        //            {
        //                var totalDays = (parent.ReturnDate.Value.Date - parent.OrderDate.Date).Days;
        //                if (totalDays > 7)
        //                    //fineAmount = (totalDays - 7) * 5;
        //                    fineAmount = 5;
        //                //i.FineAmount = fineAmount; // ₹5 per extra day
        //            }
        //             await _orderRepo.UpdateOrderItem(i);
        //            var book = bookLookup.TryGetValue(i.BookId, out var b) ? b : null;

        //            return new OrderIItemDTO
        //            {
        //                OrderId = parent.OrderId,
        //                OrderItemId = i.OrderItemId,
        //                OrderCode = parent.OrderCode,
        //                ImageUrl = i.ImageUrl ?? string.Empty,
        //                Title = i.Title,              // order item title
        //                OrderDate = parent.OrderDate,
        //                Quantity = i.Quantity,
        //                ReturnDate = parent.ReturnDate,
        //                FineAmount = fineAmount,
        //                Status = i.Status,
        //                Year = book?.Year
        //            };
        //        }).ToList();
        //        var dtoTasks = (await Task.WhenAll(dtos)).ToList();
        //        return (dtoTasks, total);
        //    }

        public async Task<UpdateOrderDTO> UpdateOrderStatus(int orderItemId)
        {
            var orderItem = await _orderRepo.GetOrderItemById(orderItemId);
            if (orderItem == null) return null;
            //var order = await _orderRepo.GetOrderById(orderItem.OrderId);
            //if (order == null) return null;
            orderItem.Status = "Approved";
            orderItem.ReturnStatus = "Pending";
            orderItem.Action = true;
            await _orderRepo.UpdateOrderItem(orderItem);
                      
            var updateOrder = new UpdateOrderDTO
            {                
                Action = orderItem.Action,
                ReturnStatus = orderItem.ReturnStatus
            };
            return updateOrder;
        }
        public async Task<UpdateOrderDTO> UpdateReturnOrderStatus(int orderItemId)
        {
            var orderItem = await _orderRepo.GetOrderItemById(orderItemId);
            if (orderItem == null) return null;
            //var order = await _orderRepo.GetOrderById(orderItem.OrderId);
            //if (order == null) return null;
            orderItem.Status = "Cleared";
            orderItem.ReturnStatus = "Returned";
            await _orderRepo.UpdateOrderItem(orderItem);
          
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
                FineAmount = orderItem.FineAmount,
                Action = orderItem.Action,
                ReturnStatus = orderItem.ReturnStatus
            };
            return updateOrder;
        }
    }
}
