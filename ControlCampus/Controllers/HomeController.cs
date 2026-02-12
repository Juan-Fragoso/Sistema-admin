using ControlCampus.Data;
using ControlCampus.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace ControlCampus.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly Connection _context;

        public HomeController(Connection context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Enrollment()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Solo para crear el primer usuario, luego borra esto
        public async Task<string> CreateAdmin()
        {
            var admin = new User
            {
                Name = "Administrador",
                Email = "admin@campus.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };  
            _context.User.Add(admin);
            await _context.SaveChangesAsync();
            return "Usuario Creado: admin@campus.com / 123456";
        }


    }
}
