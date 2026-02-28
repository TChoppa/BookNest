using BookNest.DatabaseContext;
using BookNest.Interfaces;
using BookNest.Models;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _dbContext;
        public OrderRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddOrder(Order entity)
        {
            await _dbContext.Orders.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddOrderItem(OrderItem entity)
        {
            await _dbContext.OrderItems.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<Order>> GetOrderByUsername(string username)
        {
            return await _dbContext.Orders.Where(x => x.Username == username).ToListAsync();
        }
        public async Task<List<OrderItem>> GetOrderItemByUsername(int orderId)
        {
            return await _dbContext.OrderItems.Where(x => x.OrderId == orderId).ToListAsync();
        }

    }
}
