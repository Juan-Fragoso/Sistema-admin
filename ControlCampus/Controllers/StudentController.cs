using ControlCampus.Data;
using ControlCampus.Models;
using ControlCampus.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlCampus.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private const string V = "student";
        private Connection _context;

        public StudentController(Connection context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var students = await _context.Student
                .Include(s => s.User)
                .ToListAsync();
    
            return View(students);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student, string UserName, string UserEmail, string UserPassword, long? id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                bool isEditing = id.HasValue && id > 0;
                Student? existingStudent = null;
                User? user = null;

                if (isEditing)
                {
                    existingStudent = await _context.Student.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
                    if (existingStudent == null) return NotFound();
                    user = existingStudent.User;
                }

                // Buscamos si el email existe, pero que no sea el del usuario que estamos editando
                bool emailExists = await _context.User.AnyAsync(u => u.Email == UserEmail && (!isEditing || u.Id != user.Id));

                // Buscamos si la boleta existe, pero que no sea la del estudiante que estamos editando
                bool boletaExists = await _context.Student.AnyAsync(s => s.Boleta == student.Boleta && (!isEditing || s.Id != existingStudent.Id));

                if (emailExists) ModelState.AddModelError("UserEmail", "El correo ya está registrado por otro usuario.");
                if (boletaExists) ModelState.AddModelError("Boleta", "La boleta ya está registrada en otro expediente.");


                ModelState.Remove("User");

                if (!ModelState.IsValid) return View(student);

                if (!isEditing)
                {
                    user = new User { CreatedAt = DateTime.Now };
                    _context.User.Add(user);
                }

                var newUser = new User
                {
                    Name = UserName,
                    Email = UserEmail,
                    Password = BCrypt.Net.BCrypt.HashPassword(UserPassword),
                    CreatedAt = DateTime.Now,
                };

                user.Name = UserName;
                user.Email = UserEmail;
                user.UpdatedAt = DateTime.Now;

                // Solo cambiamos contraseña si escribieron algo
                if (!string.IsNullOrEmpty(UserPassword))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(UserPassword);
                }

                if (!isEditing)
                {
                    existingStudent = student;
                    existingStudent.User = user;
                    existingStudent.CreatedAt = DateTime.Now;
                    _context.Student.Add(existingStudent);
                }
                else
                {
                    existingStudent.Boleta = student.Boleta;
                    existingStudent.Phone = student.Phone;
                    existingStudent.UpdatedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                if (!isEditing)
                {
                    var studentRole = await _context.Set<Role>()
                         .AsNoTracking()
                         .Where(r => r.Name == "student")
                         .Select(r => new { r.Id, r.Name })
                         .FirstOrDefaultAsync();

                    if (studentRole != null)
                    {
                        _context.RoleUser.Add(new RoleUser
                        {
                            UserId = user.Id,
                            RoleId = studentRole.Id,
                            CreatedAt = DateTime.Now
                        });
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
                var textMsg = isEditing ? "Actualizado correctamente" : "Creado correctamente";
                ModelState.AddModelError("Success", textMsg);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                ViewBag.UserName = UserName;
                ViewBag.UserEmail = UserEmail;

                ModelState.AddModelError("", $"Error al guardar: {ex.Message} - {ex.InnerException?.Message}");
                return View(student);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var student = await _context.Student
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View("Create", student);
        }

        public async Task<IActionResult> MyGrades()
        {
            var userEmail = User.Identity?.Name;
            var currentUser = await _context.User
                .Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser?.Student == null)
            {
                TempData["Error"] = "No tienes un perfil de estudiante asociado.";
                return RedirectToAction("Index", "Home");
            }

            var grades = await _context.Grade
                .Include(g => g.Subject)
                .Include(g => g.Enrollment)
                .Include(g => g.Teacher).ThenInclude(t => t.User) 
                .Where(g => g.Enrollment.StudentId == currentUser.Student.Id)
                .ToListAsync();

            decimal average = grades.Any()
                ? grades.Average(g => g.GradeValue ?? 0)
                : 0;

            var viewModel = new StudentGradesViewModel
            {
                Grades = grades,
                Average = Math.Round(average, 2) 
            };

            return View(viewModel);
        }
    }
}
