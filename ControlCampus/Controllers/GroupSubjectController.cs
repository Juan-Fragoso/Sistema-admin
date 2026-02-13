using ControlCampus.Data;
using ControlCampus.Models;
using ControlCampus.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlCampus.Controllers
{
    [Authorize]
    public class GroupSubjectController : Controller
    {
        private Connection _context;
        public GroupSubjectController(Connection context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity?.Name;

            var currentUser = await _context.User
                .Include(u => u.Teacher)
                .Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            IQueryable<CourseAssignment> query = _context.CourseAssignment
                .Include(c => c.Teacher).ThenInclude(t => t.User)
                .Include(c => c.Subject)
                .Include(c => c.Group);

            if (currentUser?.Teacher != null && currentUser.Student == null)
            {
                query = query.Where(c => c.TeacherId == currentUser.Teacher.Id);
            }

            var courseAssignments = await query.AsNoTracking().ToListAsync();

            return View(courseAssignments);
        }

        public async Task<IActionResult> ReadCourseAssignment(long id, string? boleta)
        {
            var assignment = await _context.CourseAssignment
                .Include(c => c.Subject)
                .Include(c => c.Group)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (assignment == null) return NotFound();

            var viewModel = new GroupSubjectDetailViewModel
            {
                CourseAssignment = assignment,
                Boleta = boleta
            };

            if (!string.IsNullOrEmpty(boleta))
            {
                // Buscamos al estudiante por Boleta
                var student = await _context.Student
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Boleta == boleta);

                if (student != null)
                {
                    viewModel.Student = student;

                    // Verificamos inscripción en ESTE grupo
                    viewModel.Enrollment = await _context.Enrollment
                        .FirstOrDefaultAsync(e => e.StudentId == student.Id && e.GroupId == assignment.GroupId);

                    if (viewModel.Enrollment != null)
                    {
                        // Buscamos si ya tiene calificación en ESTA materia
                        viewModel.Grade = await _context.Grade
                            .FirstOrDefaultAsync(g => g.EnrollmentId == viewModel.Enrollment.Id && g.SubjectId == assignment.SubjectId);
                    }
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveGrade(long EnrollmentId, long SubjectId, decimal Score, long AssignmentId)
        {
            var userEmail = User.Identity?.Name;
            var currentUser = await _context.User
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser?.Teacher == null)
            {
                TempData["Error"] = "Tu usuario no tiene un perfil de docente asociado o no tienes permisos.";
                return RedirectToAction("ReadCourseAssignment", new { id = AssignmentId });
            }

            long teacherIdActual = (long) currentUser.Teacher.Id;

            var grade = await _context.Grade
                .FirstOrDefaultAsync(g => g.EnrollmentId == EnrollmentId && g.SubjectId == SubjectId);

            if (grade == null)
            {
                grade = new Grade
                {
                    EnrollmentId = EnrollmentId,
                    SubjectId = SubjectId,
                    TeacherId = teacherIdActual,
                    GradeValue = Score,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.Grade.Add(grade);
            }
            else
            {
            
                grade.GradeValue = Score;
                grade.TeacherId = teacherIdActual; 
                grade.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Calificación guardada correctamente.";

            return RedirectToAction("ReadCourseAssignment", new { id = AssignmentId });
        }
    }


}
