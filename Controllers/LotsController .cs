using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CDM.Database;
using CDM.Database.Models;

namespace CDM2.Controllers
{
    public class LotsController : Controller
    {
        private readonly AppDbContext _context;

        public LotsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Lots
        public async Task<IActionResult> Index()
        {
            var lots = await _context.Lots
                .Include(l => l.Coproprietaire)
                .Include(l => l.SousCopropriete)
                .ToListAsync();

            return View(lots);
        }

        // GET: Lots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var lot = await _context.Lots
                .Include(l => l.Coproprietaire)
                .Include(l => l.SousCopropriete)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lot == null) return NotFound();

            return View(lot);
        }

        // GET: Lots/Create
        public IActionResult Create()
        {
            LoadSelections();
            return View();
        }

        // POST: Lots/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lot lot)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadSelections();
            return View(lot);
        }

        // GET: Lots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var lot = await _context.Lots.FindAsync(id);
            if (lot == null) return NotFound();

            LoadSelections();
            return View(lot);
        }

        // POST: Lots/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Lot lot)
        {
            if (id != lot.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Lots.Any(e => e.Id == id)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            LoadSelections();
            return View(lot);
        }

        // GET: Lots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var lot = await _context.Lots
                .Include(l => l.Coproprietaire)
                .Include(l => l.SousCopropriete)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lot == null) return NotFound();

            return View(lot);
        }

        // POST: Lots/DeleteConfirmed
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lot = await _context.Lots.FindAsync(id);
            if (lot != null)
            {
                _context.Lots.Remove(lot);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private void LoadSelections()
        {
            ViewBag.Coproprietaires =
                _context.Coproprietaires
                .Select(c => new { c.Id, NomComplet = c.Nom })
                .ToList();

            ViewBag.SousCopros =
                _context.SousCoproprietes
                .Select(s => new { s.Id, s.Nom })
                .ToList();
        }
    }
}
