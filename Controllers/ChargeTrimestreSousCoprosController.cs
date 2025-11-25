using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CDM.Database;
using CDM.Database.Models;

namespace CDM.Controllers
{
    public class ChargeTrimestreSousCoprosController : Controller
    {
        private readonly AppDbContext _context;

        public ChargeTrimestreSousCoprosController(AppDbContext context)
        {
            _context = context;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var list = await _context.ChargeTrimestreSousCopros
                .Include(c => c.ChargeTrimestre)
                .Include(c => c.SousCopropriete)
                .ToListAsync();

            return View(list);
        }

        // CREATE GET
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChargeTrimestreSousCopro cts)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(cts);
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int? chargeTrimestreId, int? sousCoproprieteId)
        {
            if (chargeTrimestreId == null || sousCoproprieteId == null)
                return NotFound();

            var assoc = await _context.ChargeTrimestreSousCopros
                .Include(c => c.ChargeTrimestre)
                .Include(c => c.SousCopropriete)
                .FirstOrDefaultAsync(c =>
                    c.ChargeTrimestreId == chargeTrimestreId &&
                    c.SousCoproprieteId == sousCoproprieteId);

            if (assoc == null)
                return NotFound();

            return View(assoc);
        }

        // DELETE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int chargeTrimestreId, int sousCoproprieteId)
        {
            var assoc = await _context.ChargeTrimestreSousCopros
                .FirstOrDefaultAsync(c =>
                    c.ChargeTrimestreId == chargeTrimestreId &&
                    c.SousCoproprieteId == sousCoproprieteId);

            if (assoc != null)
            {
                _context.ChargeTrimestreSousCopros.Remove(assoc);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Helpers
        private void LoadDropdowns()
        {
            ViewBag.Charges = _context.ChargeTrimestres
                .Include(c => c.Trimestre)
                .Select(c => new
                {
                    c.Id,
                    Label = $"{c.Libelle} ({c.Trimestre.Annee} - T{c.Trimestre.Numero})"
                })
                .ToList();

            ViewBag.SousCopros = _context.SousCoproprietes
                .OrderBy(s => s.Nom)
                .ToList();
        }
    }
}
