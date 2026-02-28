using BookNest.Models;

namespace BookNest.IServices
{
    public interface IOrderService
    {
        public  Task<string> ConfirmOrder(string username);
        public Task<List<Order>> GetOrderByUsername(string username);



    }
}
