using Microsoft.AspNetCore.Mvc;
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

        // ---------------------------------------
        // INDEX
        // ---------------------------------------
        public async Task<IActionResult> Index()
        {
            var list = await _context.ChargeTrimestres
                .Include(c => c.Trimestre)
                .Include(c => c.SousCopros)
                    .ThenInclude(sc => sc.SousCopropriete)
                .ToListAsync();

            return View(list);
        }

        // ---------------------------------------
        // DETAILS
        // ---------------------------------------
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var charge = await _context.ChargeTrimestres
                .Include(c => c.Trimestre)
                .Include(c => c.SousCopros)
                    .ThenInclude(sc => sc.SousCopropriete)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (charge == null) return NotFound();

            return View(charge);
        }

        // ---------------------------------------
        // CREATE (GET)
        // ---------------------------------------
        public IActionResult Create()
        {
            LoadSelections();
            return View();
        }

        // ---------------------------------------
        // CREATE (POST)
        // ---------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChargeTrimestre charge, int[] SousCoprosIds)
        {
            if (ModelState.IsValid)
            {
                _context.Add(charge);
                await _context.SaveChangesAsync();

                foreach (var id in SousCoprosIds)
                {
                    _context.ChargeTrimestreSousCopros.Add(new ChargeTrimestreSousCopro
                    {
                        ChargeTrimestreId = charge.Id,
                        SousCoproprieteId = id
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadSelections();
            return View(charge);
        }

        // ---------------------------------------
        // EDIT (GET)
        // ---------------------------------------
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var charge = await _context.ChargeTrimestres
                .Include(c => c.SousCopros)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (charge == null) return NotFound();

            ViewBag.SelectedSousCopros = charge.SousCopros.Select(sc => sc.SousCoproprieteId).ToList();

            LoadSelections();
            return View(charge);
        }

        // ---------------------------------------
        // EDIT (POST)
        // ---------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChargeTrimestre charge, int[] SousCoprosIds)
        {
            if (id != charge.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Update simple fields
                _context.Update(charge);
                await _context.SaveChangesAsync();

                // Reset associations
                var oldAssoc = _context.ChargeTrimestreSousCopros
                    .Where(sc => sc.ChargeTrimestreId == charge.Id);
                _context.ChargeTrimestreSousCopros.RemoveRange(oldAssoc);

                foreach (var sid in SousCoprosIds)
                {
                    _context.ChargeTrimestreSousCopros.Add(new ChargeTrimestreSousCopro
                    {
                        ChargeTrimestreId = charge.Id,
                        SousCoproprieteId = sid
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadSelections();
            return View(charge);
        }

        // ---------------------------------------
        // DELETE (GET)
        // ---------------------------------------
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var charge = await _context.ChargeTrimestres
                .Include(c => c.Trimestre)
                .Include(c => c.SousCopros)
                    .ThenInclude(sc => sc.SousCopropriete)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (charge == null) return NotFound();

            return View(charge);
        }

        // ---------------------------------------
        // DELETE (POST)
        // ---------------------------------------
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var charge = await _context.ChargeTrimestres.FindAsync(id);

            if (charge != null)
            {
                var assoc = _context.ChargeTrimestreSousCopros
                    .Where(sc => sc.ChargeTrimestreId == id);

                _context.ChargeTrimestreSousCopros.RemoveRange(assoc);
                _context.ChargeTrimestres.Remove(charge);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------
        // LOAD DROPDOWNS
        // ---------------------------------------
        private void LoadSelections()
        {
            ViewBag.Trimestres = _context.Trimestres
                .OrderBy(t => t.Annee).ThenBy(t => t.Numero)
                .Select(t => new
                {
                    t.Id,
                    Label = $"{t.Annee} - T{t.Numero}"
                })
                .ToList();

            ViewBag.SousCopros = _context.SousCoproprietes
                .OrderBy(s => s.Nom)
                .ToList();
        }
    }
}
