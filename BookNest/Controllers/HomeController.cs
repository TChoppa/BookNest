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
                return    Unauthorized(new { success = false, message = "Invalid user" });
           var isUserRegister = await _HomeService.Register(dto);
            if (isUserRegister)
                return Json(new { success = true, message = "Registration Successful" });

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
            var isUserRegister = await _HomeService.Login(dto);
            if (isUserRegister)
                return Json(new { success = true, message = "Login Successful" });

            return Json(new { success = false, message = "Invalid Credentials " });
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
            switch(isPasswordChange)
            {
                case 0: return Json(new { success = false, message = "Error While Changing Password" });
                case 1: return Json(new { success = false, message = "Both Password Should be Same" });
                case 2: return Json(new { success = false, message = "Invalid Credentials" });
                case 3: return Json(new { success = true, message = "Password Changed Successfully" });
            }           
            return Json(new { success = false, message = "Invalid Credentials " });
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
