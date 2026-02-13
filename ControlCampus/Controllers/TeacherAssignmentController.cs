using ControlCampus.Data;
using ControlCampus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ControlCampus.Controllers
{
    [Authorize]
    public class TeacherAssignmentController : Controller
    {
        private Connection _context;

        public TeacherAssignmentController(Connection context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var teacherAssignments = await _context.CourseAssignment
                .Include(e => e.Teacher)
                    .ThenInclude(t => t.User)
                .Include(s => s.Subject)
                .Include(e => e.Group)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.Teachers = await _context.Teacher
                .Include(s => s.User)
                .Select(s => new { Id = s.Id, Name = s.User.Name })
                .ToListAsync();
            ViewBag.Subjects = await _context.Subject.ToListAsync();

            ViewBag.Groups = await _context.Group.ToListAsync();

            return View(teacherAssignments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTeacherAssignment(long TeacherId, long SubjectId, long GroupId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingAssignment = await _context.CourseAssignment
                    .FirstOrDefaultAsync(c => c.TeacherId == TeacherId &&
                                             c.SubjectId == SubjectId &&
                                             c.GroupId == GroupId);

                if (existingAssignment == null)
                {
                    var newAssignment = new CourseAssignment
                    {
                        TeacherId = TeacherId,
                        SubjectId = SubjectId,
                        GroupId = GroupId,
                        CreatedAt = DateTime.Now
                    };

                    _context.CourseAssignment.Add(newAssignment);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Asignación creada correctamente.";
                }
                else
                {
              
                    TempData["Info"] = "Esta asignación ya existe.";
                }

                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Error al asignar curso: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
