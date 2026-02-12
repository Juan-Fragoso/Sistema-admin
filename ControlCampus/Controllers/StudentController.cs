using ControlCampus.Data;
using Microsoft.AspNetCore.Mvc;
using ControlCampus.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlCampus.Controllers
{
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
            // Incluimos .Include(s => s.User) para traer el nombre que vive en la tabla User
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

        // 2. Método para procesar los datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student, string UserName, string UserEmail, string UserPassword, long? id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Determinar si es Edición o Creación
                bool isEditing = id.HasValue && id > 0;
                Student? existingStudent = null;
                User? user = null;

                if (isEditing)
                {
                    existingStudent = await _context.Student.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
                    if (existingStudent == null) return NotFound();
                    user = existingStudent.User;
                }

                // 2. Validaciones con exclusión (Ignorar el ID actual)
                // Buscamos si el email existe, pero que no sea el del usuario que estamos editando
                bool emailExists = await _context.User.AnyAsync(u => u.Email == UserEmail && (!isEditing || u.Id != user.Id));

                // Buscamos si la boleta existe, pero que no sea la del estudiante que estamos editando
                bool boletaExists = await _context.Student.AnyAsync(s => s.Boleta == student.Boleta && (!isEditing || s.Id != existingStudent.Id));

                if (emailExists) ModelState.AddModelError("UserEmail", "El correo ya está registrado por otro usuario.");
                if (boletaExists) ModelState.AddModelError("Boleta", "La boleta ya está registrada en otro expediente.");


                ModelState.Remove("User");

                if (!ModelState.IsValid) return View(student);

                // 3. Lógica de Usuario (Update o Create)
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

                // 4. Lógica de Estudiante
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

                // Guardamos cambios principales
                await _context.SaveChangesAsync();

                // 5.Rol(Solo para nuevos)
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
            // Buscamos al estudiante con sus datos de usuario
            var student = await _context.Student
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            // Retornamos la vista "Create", pero le pasamos el objeto con datos
            return View("Create", student);
        }
    }
}
