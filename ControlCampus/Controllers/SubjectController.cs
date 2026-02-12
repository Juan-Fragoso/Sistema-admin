using ControlCampus.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlCampus.Controllers
{
    public class SubjectController : Controller
    {
        private readonly Connection _context;

        public SubjectController(Connection context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subject.ToListAsync();
            return View(subjects);
        }
    }
}
