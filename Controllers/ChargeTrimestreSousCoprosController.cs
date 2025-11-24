using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: ChargeTrimestreSousCopros
        public async Task<IActionResult> Index()
        {
            var list = await _context.ChargeTrimestreSousCopros
                .Include(c => c.ChargeTrimestre)
                .Include(c => c.SousCopropriete)
                .AsNoTracking()
                .OrderBy(c => c.ChargeTrimestre.Libelle)
                .ToListAsync();

            return View(list);
        }

        // GET: ChargeTrimestreSousCopros/Create
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // POST: ChargeTrimestreSousCopros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChargeTrimestreId,SousCoproprieteId")] ChargeTrimestreSousCopro link)
        {
            if (ModelState.IsValid)
            {
                _context.Add(link);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(link);
        }

        // GET: ChargeTrimestreSousCopros/Delete/5
        public async Task<IActionResult> Delete(int? chargeTrimestreId, int? sousCoproprieteId)
        {
            if (chargeTrimestreId == null || sousCoproprieteId == null)
                return NotFound();

            var link = await _context.ChargeTrimestreSousCopros
                .Include(l => l.ChargeTrimestre)
                .Include(l => l.SousCopropriete)
                .AsNoTracking()
                .FirstOrDefaultAsync(l =>
                    l.ChargeTrimestreId == chargeTrimestreId &&
                    l.SousCoproprieteId == sousCoproprieteId);

            if (link == null)
                return NotFound();

            return View(link);
        }

        // POST: ChargeTrimestreSousCopros/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int chargeTrimestreId, int sousCoproprieteId)
        {
            var link = await _context.ChargeTrimestreSousCopros.FindAsync(chargeTrimestreId, sousCoproprieteId);
            if (link != null)
            {
                _context.ChargeTrimestreSousCopros.Remove(link);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private void LoadDropdowns()
        {
            ViewBag.ChargeTrimestreId = new SelectList(
                _context.ChargeTrimestres.OrderBy(c => c.Libelle),
                "Id", "Libelle");

            ViewBag.SousCoproprieteId = new SelectList(
                _context.SousCoproprietes.OrderBy(s => s.Nom),
                "Id", "Nom");
        }
    }
}
