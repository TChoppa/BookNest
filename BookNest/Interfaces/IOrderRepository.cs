using BookNest.Models;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Interfaces
{
    public interface IOrderRepository
    {
        public Task AddOrder(Order entity);
        public Task AddOrderItem(OrderItem entity);
        public Task<List<Order>> GetAllOrders();
        // DB-side paged query with optional search on OrderCode and Username
       // public Task<(List<Order> orders, int totalCount)> GetAllOrdersPaged(int page, int pageSize, string? search);
        public Task<List<Order>> GetOrderByUsername(string username);

        public Task<List<OrderItem>> GetOrderItemByUsername(int orderId);
        public Task<OrderItem> GetOrderItemById(int orderItemId);
        public Task<Order> GetOrderById(int orderId);
        public Task DeleteOrderItem(int orderItemId);
        public  Task<Order> UpdateOrder(Order order);
        public Task UpdateOrderItem(OrderItem orderItem);
    }
        
}
