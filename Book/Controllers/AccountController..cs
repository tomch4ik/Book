using Microsoft.AspNetCore.Mvc;
using Book.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Book.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        public AccountController(AppDbContext db) => _db = db;

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string name, string pwd)
        {
            string hash = GetHash(pwd);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Name == name && u.Pwd == hash);

            if (user != null)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Name) };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Error login or password";
            return View();
        }

        [HttpGet]
        public IActionResult Registration() => View();

        [HttpPost]
        public async Task<IActionResult> Registration(string name, string pwd, string confirm)
        {
            if (pwd != confirm)
            {
                ViewBag.Error = "Passwords no match";
                return View();
            }

            if (_db.Users.Any(u => u.Name == name))
            {
                ViewBag.Error = "This login already exists";
                return View();
            }

            var user = new User { Name = name, Pwd = GetHash(pwd) };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        public IActionResult Guest() => RedirectToAction("Index", "Home");

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        private string GetHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
    }
}
