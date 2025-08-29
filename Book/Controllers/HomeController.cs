using Microsoft.AspNetCore.Mvc;
using Book.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization; 
using Microsoft.AspNetCore.Http;        

namespace Book.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repository;

        public HomeController(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var messages = await _repository.GetMessagesAsync();
            return View(messages);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddMessage(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return RedirectToAction("Index");

            var user = await _repository.GetUserByNameAsync(User.Identity.Name);
            if (user != null)
            {
                var msg = new Message
                {
                    Id_User = user.Id,
                    User = user,
                    Text = text,
                    MessageDate = DateTime.Now
                };
                await _repository.AddMessageAsync(msg);
                await _repository.SaveAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(culture))
                culture = "en"; 

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
        }
    }
}


