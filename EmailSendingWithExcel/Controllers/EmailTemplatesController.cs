using EmailSendingWithExcel.Data;
using EmailSendingWithExcel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EmailSendingWithExcel.Controllers
{
    public class EmailTemplatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmailTemplatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmailTemplates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmailTemplates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmailTemplate emailTemplate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emailTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(emailTemplate);
        }

        // GET: EmailTemplates
        public async Task<IActionResult> Index()
        {
            var templates = await _context.EmailTemplates.ToListAsync();
            return View(templates);
        }

        // GET: EmailTemplates/Edit/{id}
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var template = await _context.EmailTemplates.FindAsync(id);
            if (template == null) return NotFound();

            return View(template);
        }

        // POST: EmailTemplates/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmailTemplate emailTemplate)
        {
            if (id != emailTemplate.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(emailTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(emailTemplate);
        }

        // GET: EmailTemplates/Delete/{id}
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var template = await _context.EmailTemplates.FindAsync(id);
            if (template == null) return NotFound();

            return View(template);
        }

        // POST: EmailTemplates/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var template = await _context.EmailTemplates.FindAsync(id);
            if (template != null)
            {
                _context.EmailTemplates.Remove(template);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
