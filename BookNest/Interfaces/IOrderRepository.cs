using BookNest.Models;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Interfaces
{
    public interface IOrderRepository
    {
        public Task AddOrder(Order entity);
        public Task AddOrderItem(OrderItem entity);
        public Task<List<Order>> GetOrderByUsername(string username);
        public Task<List<OrderItem>> GetOrderItemByUsername(int orderId);
    }
        
}
