using Microsoft.AspNetCore.Mvc;
using Book.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Book.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var messages = await _db.Messages.Include(m => m.User).OrderByDescending(m => m.MessageDate).ToListAsync();
            return View(messages);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddMessage(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return RedirectToAction("Index");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Name == User.Identity.Name);
            if (user != null)
            {
                var msg = new Message
                {
                    Id_User = user.Id,
                    User = user,          
                    Text = text,
                    MessageDate = DateTime.Now
                };
                _db.Messages.Add(msg);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

    }
}
