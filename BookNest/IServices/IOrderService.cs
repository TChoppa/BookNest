using BookNest.Models;
using BookNest.DTO;

namespace BookNest.IServices
{
    public interface IOrderService
    {
        public  Task<string> ConfirmOrder(string username);
        public Task<List<Order>> GetOrderByUsername(string username);
        public Task<List<OrderIItemDTO>> GetOrderItemsByUsername(string username);



    }
}
