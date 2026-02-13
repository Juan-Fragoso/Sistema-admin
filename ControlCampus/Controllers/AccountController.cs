using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ControlCampus.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
namespace ControlCampus.Controllers
{
    public class AccountController : Controller
    {
        private readonly Connection _context;

        public AccountController(Connection context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Buscar al usuario y cargar sus roles
            var user = await _context.User
                .Include(u => u.RoleUsers)          
                    .ThenInclude(ru => ru.Role)  
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("FullName", user.Name),
                    new Claim("UserId", user.Id.ToString())
                };

            
                // Recorremos la tabla intermedia para sacar los nombres de los roles
                foreach (var ru in user.RoleUsers)
                {
                    if (ru.Role != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, ru.Role.Name));
                    }
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Crear la Cookie de sesión
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Correo o contraseña incorrectos";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}