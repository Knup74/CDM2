using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CDM.Database;
using CDM.Database.Models;

namespace CDM.Controllers
{
    public class TrimestresController : Controller
    {
        private readonly AppDbContext _context;

        public TrimestresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Trimestres
        public async Task<IActionResult> Index()
        {
            var list = await _context.Trimestres
                .AsNoTracking()
                .OrderByDescending(t => t.Annee)
                .ThenBy(t => t.Numero)
                .ToListAsync();
            return View(list);
        }

        // GET: Trimestres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var trimestre = await _context.Trimestres
                .Include(t => t.Charges)
                .Include(t => t.AppelsDeFonds)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trimestre == null) return NotFound();

            return View(trimestre);
        }

        // GET: Trimestres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trimestres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Annee,Numero,TotalPrevisionnel,TotalReel,EstValide")] Trimestre trimestre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trimestre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trimestre);
        }

        // GET: Trimestres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trimestre = await _context.Trimestres.FindAsync(id);
            if (trimestre == null) return NotFound();

            return View(trimestre);
        }

        // POST: Trimestres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Annee,Numero,TotalPrevisionnel,TotalReel,EstValide")] Trimestre trimestre)
        {
            if (id != trimestre.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trimestre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrimestreExists(trimestre.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trimestre);
        }

        // GET: Trimestres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trimestre = await _context.Trimestres
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trimestre == null) return NotFound();

            return View(trimestre);
        }

        // POST: Trimestres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trimestre = await _context.Trimestres.FindAsync(id);
            if (trimestre != null)
            {
                _context.Trimestres.Remove(trimestre);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TrimestreExists(int id)
        {
            return _context.Trimestres.Any(e => e.Id == id);
        }
    }
}
