using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CDM.Database;
using CDM.Database.Models;

namespace CDM.Controllers
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
                .AsNoTracking()
                .OrderBy(l => l.NumeroLot)
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
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lot == null) return NotFound();

            return View(lot);
        }

        // GET: Lots/Create
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // POST: Lots/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumeroLot,Tantiemes,CoproprietaireId,SousCoproprieteId")] Lot lot)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(lot);
        }

        // GET: Lots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var lot = await _context.Lots.FindAsync(id);
            if (lot == null) return NotFound();

            LoadDropdowns();
            return View(lot);
        }

        // POST: Lots/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumeroLot,Tantiemes,CoproprietaireId,SousCoproprieteId")] Lot lot)
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
                    if (!LotExists(lot.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(lot);
        }

        // GET: Lots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var lot = await _context.Lots
                .Include(l => l.Coproprietaire)
                .Include(l => l.SousCopropriete)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lot == null) return NotFound();

            return View(lot);
        }

        // POST: Lots/Delete/5
        [HttpPost, ActionName("Delete")]
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

        private void LoadDropdowns()
        {
            ViewBag.CoproprietaireId = new SelectList(
                _context.Coproprietaires.OrderBy(c => c.Nom),
                "Id", "Nom");

            ViewBag.SousCoproprieteId = new SelectList(
                _context.SousCoproprietes.OrderBy(s => s.Nom),
                "Id", "Nom");
        }

        private bool LotExists(int id)
        {
            return _context.Lots.Any(e => e.Id == id);
        }
    }
}
