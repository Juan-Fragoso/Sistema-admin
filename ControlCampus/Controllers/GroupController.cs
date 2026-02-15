using ControlCampus.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlCampus.Models;

namespace ControlCampus.Controllers
{
    public class GroupController : Controller
    {
        private readonly Connection _context;

        public GroupController(Connection context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var groups = await _context.Group.ToListAsync();
            return View(groups);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Group group)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                bool isEditing = group.Id > 0;
                Group? existingGroup = null;

                if (isEditing)
                {
                    existingGroup = await _context.Group.FindAsync(group.Id);
                    if(existingGroup == null) return NotFound();
                }

                bool groupExists = await _context.Group.AnyAsync(g => g.Name == group.Name && g.Period == group.Period && (!isEditing || g.Id != group.Id));

                if (groupExists)
                {
                    ModelState.AddModelError("Name", "Ya existe un grupo con ese nombre en el periodo ingresado");
                }

                if(!ModelState.IsValid) return View(group);

                if (!isEditing)
                {
                    _context.Group.Add(group);
                }
                else
                {
                    existingGroup!.Name = group.Name;
                    existingGroup.Period = group.Period;

                    _context.Group.Update(existingGroup);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
               
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Ocurrió inesperado " + ex.Message);
                return View(group);
            }
        }
    }
}
