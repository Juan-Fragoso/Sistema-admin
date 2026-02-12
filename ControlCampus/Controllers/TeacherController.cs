using ControlCampus.Data;
using ControlCampus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlCampus.Controllers
{
    public class TeacherController : Controller
    {
        private Connection _context;

        public TeacherController(Connection context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var teachers = await _context.Teacher
                .Include(s => s.User)
                .ToListAsync();

            return View(teachers);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher, string UserName, string UserEmail, string UserPassword, long? id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                bool isEditing = id.HasValue && id > 0;
                Teacher? existingTeacher = null;
                User? user = null;

                if (isEditing)
                {
                    existingTeacher = await _context.Teacher.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
                    if (existingTeacher == null) return NotFound();
                    user = existingTeacher.User;
                }

                // 1. Validaciones de Duplicados (Ignorando el ID actual)
                bool emailExists = await _context.User.AnyAsync(u => u.Email == UserEmail && (!isEditing || u.Id != user!.Id));
                bool employeeNumberExists = await _context.Teacher.AnyAsync(t => t.EmployeeNumber == teacher.EmployeeNumber && (!isEditing || t.Id != existingTeacher!.Id));

                if (emailExists) ModelState.AddModelError("UserEmail", "El correo ya está registrado.");
                if (employeeNumberExists) ModelState.AddModelError("EmployeeNumber", "El número de empleado ya está en uso.");

                // Quitamos la validación automática de la navegación para que no bloquee
                ModelState.Remove("User");

                if (!ModelState.IsValid) return View(teacher);

                // 2. Lógica de Usuario
                if (!isEditing)
                {
                    user = new User { CreatedAt = DateTime.Now };
                    _context.User.Add(user);
                }

                user!.Name = UserName;
                user.Email = UserEmail;
                user.UpdatedAt = DateTime.Now;

                if (!string.IsNullOrEmpty(UserPassword))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(UserPassword);
                }

                // IMPORTANTE: Guardamos para tener el ID del usuario si es nuevo
                await _context.SaveChangesAsync();

                // 3. Lógica de Maestro
                if (!isEditing)
                {
                    existingTeacher = teacher;
                    existingTeacher.UserId = user.Id;
                    existingTeacher.CreatedAt = DateTime.Now;
                    _context.Teacher.Add(existingTeacher);
                }
                else
                {
                    existingTeacher!.Phone = teacher.Phone;
                    existingTeacher.EmployeeNumber = teacher.EmployeeNumber;
                    existingTeacher.UpdatedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                // 4. Asignar Rol de Maestro (Solo si es nuevo)
                if (!isEditing)
                {
                    var teacherRole = await _context.Set<Role>().AsNoTracking().Where(r => r.Name == "teacher")
                        .Select(r => new { r.Id, r.Name })
                        .FirstOrDefaultAsync();

                    if (teacherRole != null)
                    {
                        _context.RoleUser.Add(new RoleUser
                        {
                            UserId = user.Id,
                            RoleId = teacherRole.Id,
                            CreatedAt = DateTime.Now
                        });
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
                TempData["Success"] = isEditing ? "Maestro actualizado" : "Maestro registrado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ViewBag.UserName = UserName;
                ViewBag.UserEmail = UserEmail;
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(teacher);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var teacher = await _context.Teacher
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            return View("Create", teacher);
        }

    }
}
