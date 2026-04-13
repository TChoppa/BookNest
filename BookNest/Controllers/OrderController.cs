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
        public async Task<IActionResult> GetOrderByUsername(int page = 1, int pageSize = 5, string? search = null)
        {
            var username = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { success = false, message = "User not logged in" });

            var (orders, total) = await _orderService.GetOrderItemsByUsername(username, page, pageSize, search);

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = total;
            ViewBag.Search = search ?? string.Empty;
            ViewBag.IsUserView = true;

            return View("Index", orders);
        }

        // GET: /Order/GetAllOrders?page=1&pageSize=5&search=term
        [HttpGet]
        public async Task<IActionResult> GetAllOrders(int page = 1, int pageSize = 5, string? search = null)
        {
            //ar username = HttpContext.Session.GetString("UserName");
            // call service which supports paging + search
            var (orders, total) = await _orderService.GetAllOrders(page, pageSize, search);

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = total;
            ViewBag.Search = search ?? string.Empty;
            ViewBag.IsUserView = false;
            ViewBag.IsOverdueView = true;
            ViewBag.ViewType = "all";

            return View(orders);
        }

        // AJAX endpoint for live search / pagination
        [HttpGet]


        public async Task<IActionResult> SearchOrders(string? search, int page = 1, int pageSize = 5)
        {
           // var username = HttpContext.Session.GetString("UserName");
            var (orders, total) = await _orderService.GetAllOrders(page, pageSize, search);
            return Ok(new { orders, total, page, pageSize });
        }

        // AJAX endpoint for per-user live search/paging — uses session username and service that returns user orders
        [HttpGet]
        public async Task<IActionResult> SearchOrdersForUser(string? search, int page = 1, int pageSize = 5)
        {
            var username = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { success = false, message = "User not logged in" });

            var (orders, total) = await _orderService.GetOrderItemsByUsername(username, page, pageSize, search);
            return Ok(new { orders, total, page, pageSize });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var username = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { success = false, message = "User not logged in" });

            var ok = await _orderService.DeleteOrderItemForUser(username, id);
            if (!ok)
                return BadRequest(new { success = false, message = "Unable to delete item" });

            return Ok(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderItemId)
        {
            //var username = HttpContext.Session.GetString("UserName");
            if(orderItemId <= 0)
                return BadRequest(new { success = false, message = "Invalid order item ID" });
            var res = await _orderService.UpdateOrderStatus(orderItemId);
            if (res == null)
                return Unauthorized(new { success = false, message = "Order Not Updated" });
            return Ok(new { success = true, order = res });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReturnOrderStatus(int orderItemId)
        {
            //var username = HttpContext.Session.GetString("UserName");
            if (orderItemId <= 0)
                return BadRequest(new { success = false, message = "Invalid order item ID" });
            var res = await _orderService.UpdateReturnOrderStatus(orderItemId);
            if (res == null)
                return Unauthorized(new { success = false, message = "Order Not Updated" });
            return Ok(new { success = true, order = res });
        }
        [HttpGet]
        public async Task<IActionResult> GetOverdueNotClearedOrders(int page = 1, int pageSize = 10, string? search = null)
        {
            var (data, total) = await _orderService.GetOverdueNotClearedOrders(page, pageSize, search);

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = total;
            ViewBag.Search = search ?? string.Empty;
            ViewBag.ViewType = "overdue";// or true if this is a per-user view
            //return View(data);
            return View("GetAllOrders", data);
        }
        [HttpGet]
        public async Task<IActionResult> GetOverdueNotClearedOrdersData(int page = 1, int pageSize = 10, string? search = null)
        {
            var (data, total) = await _orderService.GetOverdueNotClearedOrders(page, pageSize, search);

            return Json(new
            {
                orders = data,
                total = total
            });
        }

    }
}
