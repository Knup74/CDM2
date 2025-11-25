using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CDM.Database;
using CDM.Database.Models;

namespace CDM.Controllers
{
    [AdminOnly]
    public class CoproprietairesController : Controller
    {
        private readonly AppDbContext _context;

        public CoproprietairesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Coproprietaires
        public async Task<IActionResult> Index()
        {
            var list = await _context.Coproprietaires
                .AsNoTracking()
                .OrderBy(c => c.Nom)
                .ToListAsync();
            return View(list);
        }

        // GET: Coproprietaires/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var copro = await _context.Coproprietaires
                .Include(c => c.Lots)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (copro == null) return NotFound();

            return View(copro);
        }

        // GET: Coproprietaires/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coproprietaires/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nom, Email")] Coproprietaire copro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(copro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(copro);
        }

        // GET: Coproprietaires/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var copro = await _context.Coproprietaires.FindAsync(id);
            if (copro == null) return NotFound();

            return View(copro);
        }

        // POST: Coproprietaires/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Nom, Email")] Coproprietaire copro)
        {
            if (id != copro.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(copro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoproprietaireExists(copro.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(copro);
        }

        // GET: Coproprietaires/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var copro = await _context.Coproprietaires
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (copro == null) return NotFound();

            return View(copro);
        }

        // POST: Coproprietaires/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var copro = await _context.Coproprietaires.FindAsync(id);
            if (copro != null)
            {
                _context.Coproprietaires.Remove(copro);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CoproprietaireExists(int id)
        {
            return _context.Coproprietaires.Any(e => e.Id == id);
        }
    }
}
