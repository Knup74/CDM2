using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: AppelsDeFonds
        public async Task<IActionResult> Index()
        {
            var list = await _context.AppelsDeFonds
                .Include(a => a.Trimestre)
                .Include(a => a.Coproprietaire)
                .AsNoTracking()
                .ToListAsync();

            return View(list);
        }

        // GET: AppelsDeFonds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var appel = await _context.AppelsDeFonds
                .Include(a => a.Trimestre)
                .Include(a => a.Coproprietaire)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appel == null)
                return NotFound();

            return View(appel);
        }

        // GET: AppelsDeFonds/Create
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // POST: AppelsDeFonds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppelDeFond appel)
        {
            if (ModelState.IsValid)
            {
                // auto-calcul si besoin
                appel.Regularisation = appel.MontantRegle - appel.MontantDu;

                _context.Add(appel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(appel);
        }

        // GET: AppelsDeFonds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var appel = await _context.AppelsDeFonds.FindAsync(id);
            if (appel == null)
                return NotFound();

            LoadDropdowns();
            return View(appel);
        }

        // POST: AppelsDeFonds/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppelDeFond appel)
        {
            if (id != appel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    appel.Regularisation = appel.MontantRegle - appel.MontantDu;

                    _context.Update(appel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppelExists(appel.Id))
                        return NotFound();

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(appel);
        }

        // GET: AppelsDeFonds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var appel = await _context.AppelsDeFonds
                .Include(a => a.Trimestre)
                .Include(a => a.Coproprietaire)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appel == null)
                return NotFound();

            return View(appel);
        }

        // POST: AppelsDeFonds/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
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

        private void LoadDropdowns()
        {
            ViewBag.TrimestreId = new SelectList(
                _context.Trimestres
                    .OrderBy(t => t.Annee)
                    .ThenBy(t => t.Numero)
                    .Select(t => new {
                        t.Id,
                        Label = t.Annee + " - T" + t.Numero
                    }),
                "Id", "Label"
            );

            ViewBag.CoproprietaireId = new SelectList(
                _context.Coproprietaires.OrderBy(c => c.Nom),
                "Id", "Nom"
            );
        }

        private bool AppelExists(int id)
        {
            return _context.AppelsDeFonds.Any(e => e.Id == id);
        }
    }
}
