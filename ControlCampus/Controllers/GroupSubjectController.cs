using ControlCampus.Data;
using ControlCampus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlCampus.Controllers
{
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
    }
}
