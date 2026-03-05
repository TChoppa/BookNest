using BookNest.IServices;
using BookNest.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookNest.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmOrder()
        {
            var username = HttpContext.Session.GetString("UserName");
            var res = await _orderService.ConfirmOrder(username);
            if (res == null)
                return Unauthorized(new { success = false, message = "Order Not Confirmed" });
            return Ok(new { success = true, orderId = res });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderByUsername()
         {
            var username = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { success = false, message = "User not logged in" });

            var orders = await _orderService.GetOrderItemsByUsername(username);
            Console.WriteLine($"Orders count: {orders?.Count}");
            return View("Index", orders);
        }

    }
}
