using ControlCampus.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlCampus.Controllers
{
    public class GroupController : Controller
    {
        private readonly Connection _context;

        public GroupController(Connection context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Consulta simple al catálogo de grupos
            var groups = await _context.Group.ToListAsync();
            return View(groups);
        }
    }
}
