using ControlCampus.Data;
using ControlCampus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlCampus.Controllers
{
    public class UserController : Controller
    {
        private readonly Connection _context;

        public UserController(Connection context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Traemos el usuario + sus roles para mostrarlos en la tabla
            var users = await _context.User
                .Include(u => u.RoleUsers)
                    .ThenInclude(ru => ru.Role)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return View(users);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewBag.Roles = _context.Role.ToList();

            return View();
        } 

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, long selectedRoleId)
        {
            if (ModelState.IsValid)
            {
                // Encriptamos la contraseña antes de guardar
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;

                _context.Add(user);
                await _context.SaveChangesAsync();

                // 3. Crear la relación en RoleUser (Asignando Rol ID 2 por defecto)
                var userRole = new RoleUser
                {
                    UserId = user.Id, // El ID ya existe gracias al SaveChanges anterior
                    RoleId = selectedRoleId,      
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.RoleUser.Add(userRole);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();
            var user = await _context.User.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }


    }
}
