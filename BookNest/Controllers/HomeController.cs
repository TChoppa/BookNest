using BookNest.DTO;
using BookNest.Interfaces;
using BookNest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BookNest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHome _HomeService;

        
        public HomeController(ILogger<HomeController> logger , IHome homeserive)
        {
            _logger = logger;
            _HomeService = homeserive;  
        }

        public IActionResult Index()
        {
            ViewBag.PageTitle = "DashBoard";
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            ViewBag.PageTitle = "Register";
           if(dto== null)
                return    Unauthorized(new { success = false, message = "Invalid Credentials" });
           var isUserRegister = await _HomeService.Register(dto);
            if (isUserRegister)
                return Json(new { success = true, message = "Registration Successful" , redirectUrl = Url.Action("Login", "Home") });

            return Json(new { success = false, message = "Duplicate User " });
        }
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.PageTitle = "Login";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO dto)
            {
            ViewBag.PageTitle = "Login";
            if (dto == null)
                return Unauthorized(new { success = false, message = "Invalid user" });
            var userExists = await _HomeService.Login(dto);
            if(userExists==null)
                return Json(new { success = false, message = "Invalid user" });
            HttpContext.Session.SetString("UserName", userExists.Username);
            string roleMessage = userExists.Fk_RoleId switch
            {             
                1 => "Login Student successfully",
                2 => "Login Faculty successfully",
                3 => "Login Librarian successfully",
                4 => "Login Admin successfully",
                _ => "Login Successfullty"
            };
            return Json(new { success = true, message = roleMessage, redirectUrl = Url.Action("Index", "CartControllercs") });
        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            ViewBag.PageTitle = "ForgetPassword";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPassword dto)
        {
            ViewBag.PageTitle = "ForgetPassword";
            if (dto == null)
                return Unauthorized(new { success = false, message = "Invalid user" });
            var isPasswordChange = await _HomeService.EditPasssword(dto);
            var result = isPasswordChange switch
            {
                0 => new { success = false, message = "Error While Changing Password" },
                1 => new { success = false, message = "Both Password Should be Same" },
                2 => new { success = false, message = "Invalid Credentials" },
                3 => new { success = true, message = "Password Changed Successfully" },
                _ => new { success = false, message = "Invalid Credentials" }
            };

            return Json(result);

            //switch (isPasswordChange)
            //{
            //    case 0: return Json(new { success = false, message = "Error While Changing Password" });
            //    case 1: return Json(new { success = false, message = "Both Password Should be Same" });
            //    case 2: return Json(new { success = false, message = "Invalid Credentials" });
            //    case 3: return Json(new { success = true, message = "Password Changed Successfully" });
            //}           
            //return Json(new { success = true, message = pswdMessage });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
