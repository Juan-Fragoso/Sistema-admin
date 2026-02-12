using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlCampus.Models;
using ControlCampus.Data;

namespace ControlCampus.Controllers
{
    public class RoleController : Controller
    {
        private readonly Connection _context;

        public RoleController(Connection context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _context.Role.ToListAsync();
            return View(roles);
        }
    }
}
