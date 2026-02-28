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
                return Unauthorized(new { success = false, message = "" });
            return Ok(new { success = true, orderId = res });
        }

    }
}
