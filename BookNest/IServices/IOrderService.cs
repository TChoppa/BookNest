using BookNest.Models;
using BookNest.DTO;

namespace BookNest.IServices
{
    public interface IOrderService
    {
        public  Task<string> ConfirmOrder(string username);
        //public Task<(List<OrderIItemDTO> orders, int totalCount)> GetAllOrders(int page, int pageSize, string? search);
        public  Task<(List<OrderIItemDTO> orderItems, int totalCount)> GetAllOrders(int page, int pageSize, string? search);

        //public async Task<(List<OrderIItemDTO> orderItems, int totalCount)> GetAllOrderItems(int page, int pageSize, string? search)
        public Task<List<OrderIItemDTO>> GetOrderItemsByUsername(string username);
        // paged version for per-user orders with optional search
        public Task<(List<OrderIItemDTO> orders, int totalCount)> GetOrderItemsByUsername(string username, int page, int pageSize, string? search);
        // delete an order item belonging to a specific user
        public Task<bool> DeleteOrderItemForUser(string username, int orderItemId);
        public Task<UpdateOrderDTO> UpdateOrderStatus(int orderItemId);

        public Task<UpdateOrderDTO> UpdateReturnOrderStatus(int orderItemId);
        public Task<(List<OrderIItemDTO> data, int totalCount)>
    GetOverdueNotClearedOrders(int page, int pageSize, string? search);
        public Task<List<OrderItem>> GetAllOrderItems();

    }
}
