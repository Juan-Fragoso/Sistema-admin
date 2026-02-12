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
            // 1. Obtener el Email o ID del usuario autenticado (desde el Cookie/Token)
            var userEmail = User.Identity?.Name;

            // 2. Buscar al usuario en la BD con sus perfiles (Teacher/Student)
            var currentUser = await _context.User
                .Include(u => u.Teacher)
                .Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            // Definimos la consulta base
            IQueryable<CourseAssignment> query = _context.CourseAssignment
                .Include(c => c.Teacher).ThenInclude(t => t.User)
                .Include(c => c.Subject)
                .Include(c => c.Group);

            // 3. Aplicar la lógica de filtrado (Igual que en PHP)
            if (currentUser?.Teacher != null && currentUser.Student == null)
            {
                // Es maestro: solo ve sus asignaciones
                query = query.Where(c => c.TeacherId == currentUser.Teacher.Id);
            }
            else if (currentUser?.Student == null && currentUser?.Teacher == null)
            {
                // Es Administrador (no es ni alumno ni maestro): ve todo
                // No aplicamos ningún Where, se queda con el .all() implícito
            }

            var courseAssignments = await query.AsNoTracking().ToListAsync();

            return View(courseAssignments);
        }

        public async Task<IActionResult> ReadCourseAssignment(long id, string? boleta)
        {
            // 1. Buscamos la asignación (el grupo y materia)
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
                // 2. Buscamos al estudiante por Boleta
                var student = await _context.Student
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Boleta == boleta);

                if (student != null)
                {
                    viewModel.Student = student;

                    // 3. Verificamos inscripción en ESTE grupo
                    viewModel.Enrollment = await _context.Enrollment
                        .FirstOrDefaultAsync(e => e.StudentId == student.Id && e.GroupId == assignment.GroupId);

                    if (viewModel.Enrollment != null)
                    {
                        // 4. Buscamos si ya tiene calificación en ESTA materia
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
            // 1. Obtener el usuario actual con su perfil de docente
            var userEmail = User.Identity?.Name;
            var currentUser = await _context.User
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            // 2. Validación de Seguridad: Si no es docente o el objeto es nulo
            if (currentUser?.Teacher == null)
            {
                TempData["Error"] = "Tu usuario no tiene un perfil de docente asociado o no tienes permisos.";
                return RedirectToAction("ReadCourseAssignment", new { id = AssignmentId });
            }

            // 3. Extraer el ID a una variable local para evitar errores de nulabilidad
            // Al estar dentro de este bloque, C# ya sabe que Teacher no es nulo
            long teacherIdActual = (long) currentUser.Teacher.Id;

            // 4. Buscar si ya existe una calificación previa
            var grade = await _context.Grade
                .FirstOrDefaultAsync(g => g.EnrollmentId == EnrollmentId && g.SubjectId == SubjectId);

            if (grade == null)
            {
                // Crear nuevo registro
                grade = new Grade
                {
                    EnrollmentId = EnrollmentId,
                    SubjectId = SubjectId,
                    TeacherId = teacherIdActual, // Usamos la variable local segura
                    GradeValue = Score,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.Grade.Add(grade);
            }
            else
            {
                // Actualizar registro existente
                grade.GradeValue = Score;
                grade.TeacherId = teacherIdActual; // Actualizamos quién puso la última nota
                grade.UpdatedAt = DateTime.Now;
            }

            // 5. Guardar cambios
            await _context.SaveChangesAsync();

            // 6. Mensaje de éxito
            TempData["Success"] = "Calificación guardada correctamente.";

            return RedirectToAction("ReadCourseAssignment", new { id = AssignmentId });
        }
    }


}
