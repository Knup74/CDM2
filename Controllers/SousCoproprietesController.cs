using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CDM.Database;
using CDM.Database.Models;

namespace CDM.Controllers
{
    public class SousCoproprietesController : Controller
    {
        private readonly AppDbContext _context;

        public SousCoproprietesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: SousCoproprietes
        public async Task<IActionResult> Index()
        {
            var list = await _context.SousCoproprietes
                .Include(s => s.Lots)
                .AsNoTracking()
                .OrderBy(s => s.Nom)
                .ToListAsync();

            return View(list);
        }

        // GET: SousCoproprietes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sousCopro = await _context.SousCoproprietes
                .Include(s => s.Lots)
                .Include(s => s.ChargeAssociations)
                    .ThenInclude(ca => ca.ChargeTrimestre)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sousCopro == null) return NotFound();

            return View(sousCopro);
        }

        // GET: SousCoproprietes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SousCoproprietes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nom")] SousCopropriete sousCopro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sousCopro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sousCopro);
        }

        // GET: SousCoproprietes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sousCopro = await _context.SousCoproprietes.FindAsync(id);
            if (sousCopro == null) return NotFound();

            return View(sousCopro);
        }

        // POST: SousCoproprietes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom")] SousCopropriete sousCopro)
        {
            if (id != sousCopro.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sousCopro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SousCoproExists(sousCopro.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(sousCopro);
        }

        // GET: SousCoproprietes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sousCopro = await _context.SousCoproprietes
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sousCopro == null) return NotFound();

            return View(sousCopro);
        }

        // POST: SousCoproprietes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sousCopro = await _context.SousCoproprietes.FindAsync(id);
            if (sousCopro != null)
            {
                _context.SousCoproprietes.Remove(sousCopro);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SousCoproExists(int id)
        {
            return _context.SousCoproprietes.Any(e => e.Id == id);
        }
    }
}
