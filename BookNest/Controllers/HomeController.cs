using BookNest.DTO;
using BookNest.Interfaces;
using BookNest.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Security.Claims;
using BookNest.Enums;
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

            if (dto == null)
                return BadRequest(new { success = false, message = "Invalid request data" });

            try
            {
                var isNotUserRegister = await _HomeService.Register(dto);

                if (isNotUserRegister)
                    return Ok(new
                    {
                        success = true,
                        message = "Registration Successful",
                        redirectUrl = Url.Action("Login", "Home")
                    });

                return Unauthorized(new { success = false, message = "Duplicate User" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
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
                return BadRequest(new { success = false, message = "Invalid user" });
            var userExists = await _HomeService.Login(dto);
            if(userExists==null)
                return Unauthorized (new { success = false, message = "User Not Exists" });
            var userRole = await _HomeService.GetAllRoles(userExists.Fk_RoleId);
            if (userRole==null)
                return Unauthorized(new { success = false, message = "Invalid user" });
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userExists.Username),
                new Claim(ClaimTypes.Email, userExists.Email),
                new Claim(ClaimTypes.Role, userRole.Name)
            };
            var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                });

            HttpContext.Session.SetString("UserName", userExists.Username);
            string roleMessage = userExists.Fk_RoleId switch
            {             
                1 => "Login Student successfully",
                2 => "Login Faculty successfully",
                3 => "Login Librarian successfully",
                4 => "Login Admin successfully",
                _ => "Login Successfullty"
            };
            return Ok(new { success = true, message = roleMessage, redirectUrl = Url.Action("Index", "CartControllercs") });
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
                return BadRequest(new { success = false, message = "Invalid user" });
            var isPasswordChange = await _HomeService.EditPasssword(dto);
            var result = isPasswordChange switch
            {
                PasswordChange.UnChanged => new { success = false, message = "Error While Changing Password" },
                PasswordChange.PasswordMismatch => new { success = false, message = "Both Password Should be Same" },
                PasswordChange.DuplicatePassword => new { success = false, message = "Previous Password is same" },
                PasswordChange.Success => new { success = true, message = "Password Changed Successfully" },
                _ => new { success = false, message = "Invalid Credentials" }
            };

            return Ok(result);

        }

        public async Task<IActionResult> Dashboard()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var users = await _HomeService.GetUserByUsername(userName);
            ViewBag.roleId = users.Fk_RoleId;
            return View();
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
