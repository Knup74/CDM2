using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CDM.Database;
using CDM.Database.Models;

namespace CDM.Controllers
{
    public class ChargeTrimestresController : Controller
    {
        private readonly AppDbContext _context;

        public ChargeTrimestresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Charges
        public async Task<IActionResult> Index()
        {
            var charges = await _context.ChargeTrimestres
                .Include(c => c.Trimestre)
                .Include(c => c.SousCopros)
                    .ThenInclude(link => link.SousCopropriete)
                .AsNoTracking()
                .OrderByDescending(c => c.Trimestre.Annee)
                .ThenBy(c => c.Trimestre.Numero)
                .ToListAsync();

            return View(charges);
        }

        // GET: ChargeTrimestres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var charge = await _context.ChargeTrimestres
                .Include(c => c.Trimestre)
                .Include(c => c.SousCopros)
                    .ThenInclude(link => link.SousCopropriete)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (charge == null) return NotFound();

            return View(charge);
        }

        // GET: ChargeTrimestres/Create
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // POST: ChargeTrimestres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChargeTrimestre charge, int[] sousCoproIds)
        {
            if (ModelState.IsValid)
            {
                foreach (var id in sousCoproIds)
                {
                    charge.SousCopros.Add(new ChargeTrimestreSousCopro
                    {
                        SousCoproprieteId = id
                    });
                }

                _context.Add(charge);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(charge);
        }

        // GET: ChargeTrimestres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var charge = await _context.ChargeTrimestres
                .Include(c => c.SousCopros)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (charge == null) return NotFound();

            LoadDropdowns(charge);
            return View(charge);
        }

        // POST: ChargeTrimestres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChargeTrimestre charge, int[] sousCoproIds)
        {
            if (id != charge.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Charge existante
                    var existing = await _context.ChargeTrimestres
                        .Include(c => c.SousCopros)
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (existing == null) return NotFound();

                    existing.Libelle = charge.Libelle;
                    existing.MontantPrevisionnel = charge.MontantPrevisionnel;
                    existing.MontantReel = charge.MontantReel;
                    existing.TrimestreId = charge.TrimestreId;

                    // On met à jour les liens
                    existing.SousCopros.Clear();
                    foreach (var sid in sousCoproIds)
                    {
                        existing.SousCopros.Add(new ChargeTrimestreSousCopro
                        {
                            SousCoproprieteId = sid,
                            ChargeTrimestreId = existing.Id
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChargeExists(charge.Id)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns(charge);
            return View(charge);
        }

        // GET: ChargeTrimestres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var charge = await _context.ChargeTrimestres
                .Include(c => c.Trimestre)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (charge == null) return NotFound();

            return View(charge);
        }

        // POST: ChargeTrimestres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var charge = await _context.ChargeTrimestres.FindAsync(id);
            if (charge != null)
            {
                _context.ChargeTrimestres.Remove(charge);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ChargeExists(int id)
        {
            return _context.ChargeTrimestres.Any(e => e.Id == id);
        }

        private void LoadDropdowns(ChargeTrimestre? charge = null)
        {
            ViewBag.TrimestreId = new SelectList(
                _context.Trimestres.OrderBy(t => t.Annee).ThenBy(t => t.Numero),
                "Id",
                "Numero",
                charge?.TrimestreId);

            ViewBag.SousCopros = new MultiSelectList(
                _context.SousCoproprietes.OrderBy(s => s.Nom),
                "Id",
                "Nom",
                charge?.SousCopros.Select(x => x.SousCoproprieteId));
        }
    }
}
