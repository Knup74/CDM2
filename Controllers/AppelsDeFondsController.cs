using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CDM.Database;
using CDM.Database.Models;

namespace CDM.Controllers
{
    public class AppelsDeFondsController : Controller
    {
        private readonly AppDbContext _context;

        public AppelsDeFondsController(AppDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------
        // INDEX
        // ---------------------------------------
        public async Task<IActionResult> Index()
        {
            var appels = await _context.AppelsDeFonds
                .Include(a => a.Trimestre)
                .Include(a => a.Coproprietaire)
                .ToListAsync();

            return View(appels);
        }

        // ---------------------------------------
        // DETAILS
        // ---------------------------------------
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var appel = await _context.AppelsDeFonds
                .Include(a => a.Trimestre)
                .Include(a => a.Coproprietaire)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appel == null)
                return NotFound();

            return View(appel);
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
        public async Task<IActionResult> Create(AppelDeFond appel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadSelections();
            return View(appel);
        }

        // ---------------------------------------
        // EDIT (GET)
        // ---------------------------------------
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var appel = await _context.AppelsDeFonds.FindAsync(id);
            if (appel == null)
                return NotFound();

            LoadSelections();
            return View(appel);
        }

        // ---------------------------------------
        // EDIT (POST)
        // ---------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppelDeFond appel)
        {
            if (id != appel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(appel);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            LoadSelections();
            return View(appel);
        }

        // ---------------------------------------
        // DELETE (GET)
        // ---------------------------------------
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var appel = await _context.AppelsDeFonds
                .Include(a => a.Trimestre)
                .Include(a => a.Coproprietaire)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appel == null)
                return NotFound();

            return View(appel);
        }

        // ---------------------------------------
        // DELETE (POST)
        // ---------------------------------------
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appel = await _context.AppelsDeFonds.FindAsync(id);

            if (appel != null)
            {
                _context.AppelsDeFonds.Remove(appel);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------
        // Load dropdowns
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

            ViewBag.Coproprietaires = _context.Coproprietaires
                .OrderBy(c => c.Nom)
                .ToList();
        }
    }
}
