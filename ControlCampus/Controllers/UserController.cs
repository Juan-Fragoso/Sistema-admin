using ControlCampus.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlCampus.Controllers
{
    public class UserController : Controller
    {
        private readonly Connection _context;

        public UserController(Connection context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Traemos el usuario + sus roles para mostrarlos en la tabla
            var users = await _context.User
                .Include(u => u.RoleUsers)
                    .ThenInclude(ru => ru.Role)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return View(users);
        }
    }
}
