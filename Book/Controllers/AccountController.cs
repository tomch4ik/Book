using Microsoft.AspNetCore.Mvc;
using Book.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Book.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Book.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRepository _repository;
        private readonly IPasswordService _passwordService;

        public AccountController(IRepository repository, IPasswordService passwordService)
        {
            _repository = repository;
            _passwordService = passwordService;
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _repository.GetUserByNameAsync(model.Name.Trim());
            if (user == null)
            {
                ModelState.AddModelError("", "Пользователь не найден");
                return View(model);
            }

            var hash = _passwordService.HashPassword(model.Password, user.Salt);
            if (hash != user.Pwd)
            {
                ModelState.AddModelError("", "Неверный пароль");
                return View(model);
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Name) };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Registration() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _repository.UserExistsAsync(model.Name.Trim()))
            {
                ModelState.AddModelError("Name", "Этот логин уже существует");
                return View(model);
            }

            var salt = _passwordService.GenerateSalt();
            var user = new User
            {
                Name = model.Name.Trim(),
                Salt = salt,
                Pwd = _passwordService.HashPassword(model.Password, salt)
            };

            await _repository.AddUserAsync(user);
            await _repository.SaveAsync();

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
