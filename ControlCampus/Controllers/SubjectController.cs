using ControlCampus.Data;
using ControlCampus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlCampus.Controllers
{
    public class SubjectController : Controller
    {
        private readonly Connection _context;

        public SubjectController(Connection context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subject.ToListAsync();
            return View(subjects);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subject subject)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                bool isEditing = subject.Id > 0;
                Subject? existingSubject = null;

                if (isEditing)
                {
                    existingSubject = await _context.Subject.FindAsync(subject.Id);
                    if (existingSubject == null) return NotFound();
                }

                bool subjectExists = await _context.Subject.AnyAsync(s => s.Name == subject.Name && s.Code == subject.Code && (!isEditing || s.Id != subject.Id));

                if (subjectExists)
                {
                    ModelState.AddModelError("Name", "Ya existe esa materia con ese nombre");
                }

                if (!ModelState.IsValid) return View(subject);

                if (!isEditing)
                {
                    _context.Subject.Add(subject);
                }
                else
                {
                    existingSubject!.Name = subject.Name;
                    existingSubject.Code = subject.Code;
                    existingSubject.Credits = subject.Credits;

                    _context.Subject.Update(existingSubject);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Ocurrio un error inesperado" + ex.Message);
                return View(subject);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var subject = await _context.Subject.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            return View("Create", subject);
        }
    }
}
