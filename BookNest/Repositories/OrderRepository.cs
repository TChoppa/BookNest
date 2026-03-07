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
        public async Task<List<Order>> GetAllOrders()
        {
            return await _dbContext.Orders.ToListAsync();
        }
        public async Task<List<Order>> GetOrderByUsername(string username)
        {
            return await _dbContext.Orders.Where(x => x.Username == username).ToListAsync();
        }
        public async Task<List<OrderItem>> GetOrderItemByUsername(int orderId)
        {
            return await _dbContext.OrderItems.Where(x => x.OrderId == orderId).ToListAsync();
        }

        public async Task<OrderItem> GetOrderItemById(int orderItemId)
        {
            return await _dbContext.OrderItems.FirstOrDefaultAsync(i => i.OrderItemId == orderItemId);
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
        public async Task<Order> UpdateOrder(Order order)
        {
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
            return order;
        }
        public async Task DeleteOrderItem(int orderItemId)
        {
            // Remove the order item and delete the parent order if it has no more items
            var item = await _dbContext.OrderItems.FirstOrDefaultAsync(i => i.OrderItemId == orderItemId);
            if (item == null) return;

            // Use a transaction to ensure atomicity
            using (var tx = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var book = await _dbContext.Books.Where(x => x.BookId == item.BookId).FirstOrDefaultAsync();
                    if (book == null) return;
                    book.AvailableQuantity += item.Quantity;
                    _dbContext.Books.Update(book);
                    await _dbContext.SaveChangesAsync();

                    var orderId = item.OrderId;
                    _dbContext.OrderItems.Remove(item);
                    await _dbContext.SaveChangesAsync();

                    // if no remaining items for the order, remove the order as well
                    var remaining = await _dbContext.OrderItems.AnyAsync(i => i.OrderId == orderId);
                    if (!remaining)
                    {
                        var parent = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                        if (parent != null)
                        {
                            _dbContext.Orders.Remove(parent);
                            await _dbContext.SaveChangesAsync();
                        }
                    }

                    await tx.CommitAsync();
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
            }
        }

    }
}
