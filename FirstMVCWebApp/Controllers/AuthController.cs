using FirstMVCWebApp.Data;
using FirstMVCWebApp.Dto;
using FirstMVCWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstMVCWebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserDTO dto)
        {
            if (dto == null) return View();

            var user = _db.Users.FirstOrDefault(u => u.Email == dto.Email && u.Password == dto.Password);

            if (user == null)
            {
                ViewBag.Message = "Invalid Email or Password";
                return View(dto);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("Username", user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Store the authenticated user's email in the session for layout/navbar rendering.
            HttpContext.Session.SetString("UserEmail", user.Email);

            // Redirect the user to the Product catalog landing page upon successful login.
            return RedirectToAction("Index", "Product");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(UserDTO u)
        {
            if (!ModelState.IsValid) return View(u);

            if (_db.Users.Any(x => x.Email == u.Email))
            {
                ViewBag.Message = "This email is already registered!";
                return View(u);
            }

            var newUser = new User
            {
                Username = u.UserName,
                Email = u.Email,
                Password = u.Password
            };

            _db.Users.Add(newUser);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful!";
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Product");
        }
    }
}