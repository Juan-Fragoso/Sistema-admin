using ControlCampus.Data;
using ControlCampus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ControlCampus.Controllers
{
    [Authorize]
    public class EnrollmentController : Controller
    {
        private Connection _context;

        public EnrollmentController(Connection context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var enrollments = await _context.Enrollment
                .Include(e => e.Student)
                    .ThenInclude(s => s.User)
                .Include(e => e.Group)
                .AsNoTracking()
                .ToListAsync();

            // 2. Traemos los catálogos para los selects
            ViewBag.Students = await _context.Student
                .Include(s => s.User)
                .Select(s => new { Id = s.Id, Name = s.User.Name })
                .ToListAsync();

            ViewBag.Groups = await _context.Group.ToListAsync();

            return View(enrollments);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEnrollment(long StudentId, long GroupId)
        {
            // Validar si ya existe la inscripción para no duplicar
            bool exists = await _context.Enrollment
                .AnyAsync(e => e.StudentId == StudentId && e.GroupId == GroupId);

            if (exists)
            {
                TempData["Error"] = "El estudiante ya está inscrito en este grupo.";
                return RedirectToAction(nameof(Index));
            }

            var newEnrollment = new Enrollment
            {
                StudentId = StudentId,
                GroupId = GroupId,
                Status = "enrolled"
            };

            _context.Enrollment.Add(newEnrollment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Inscripción realizada con éxito";
            return RedirectToAction(nameof(Index));
        }
    }
}
