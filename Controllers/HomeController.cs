using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CDM.Database;
using CDM.Database.Models;
using CDM.Models;
using System;

namespace CDM.Web.Controllers
{
    [RequireLogin]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var vm = new DashboardViewModel();
            vm.CoproprietaireId = userId.Value;

            // Récupérer copro
            var copro = await _context.Coproprietaires
                .Include(c => c.Lots)
                    .ThenInclude(l => l.SousCopropriete)
                .FirstOrDefaultAsync(c => c.Id == userId.Value);

            if (copro == null) return RedirectToAction("Login", "Auth");

            vm.CoproprietaireNom = copro.Nom;

            // Totaux tantiemes (pour répartition proportionnelle)
            var totalTantiemes = (await _context.Lots
                                .Select(l => (decimal)l.Tantiemes)
                                .ToListAsync())
                                .DefaultIfEmpty(0m)
                                .Sum();
            var coproTantiemes = copro.Lots.Sum(l => l.Tantiemes);

            // --- KPI & Appels ---
            // trimestre courant
            var now = DateTime.UtcNow;
            var currentYear = now.Year;
            // déterminer trimestre courant simple:
            var month = now.Month;
            var currentQuarter = (month - 1) / 3 + 1;

            // Appels de fonds du copro
            var appels = await _context.AppelsDeFonds
                .Where(a => a.CoproprietaireId == userId.Value)
                .Include(a => a.Trimestre)
                .OrderByDescending(a => a.Trimestre.Annee)
                .ThenByDescending(a => a.Trimestre.Numero)
                .ToListAsync();

            vm.AppelsHistory = appels.Select(a => new AppelSummaryItem {
                Id = a.Id,
                Annee = a.Trimestre.Annee,
                TrimestreNumero = a.Trimestre.Numero,
                MontantDu = a.MontantDu,
                MontantRegle = a.MontantRegle,
                Regularisation = a.Regularisation,
                CreatedAt = null
            }).ToList();

            vm.LastAppel = vm.AppelsHistory.FirstOrDefault() ?? new AppelSummaryItem();

            // KPI current quarter (somme des appels pour ce trimestre)
            var currentAppels = appels
                .Where(a => a.Trimestre.Annee == currentYear && a.TrimestreId == currentQuarter);

            vm.MontantDuCurrentQuarter = currentAppels.Sum(a => a.MontantDu);
            vm.MontantRegleCurrentQuarter = currentAppels.Sum(a => a.MontantRegle);

            // Solde cumulatif (depuis tout l'historique) -> choix 3:C
            vm.SoldeCumulatif = vm.AppelsHistory.Sum(x => x.MontantDu - x.MontantRegle);

            // --- Lots summary ---
            vm.Lots = copro.Lots.Select(l => new LotSummary {
                Id = l.Id,
                NumeroLot = l.NumeroLot,
                Tantiemes = l.Tantiemes,
                SousCopro = l.SousCopropriete?.Nom ?? string.Empty
            }).ToList();

            // --- Evolution (per trimestre) pour année courante, mais uniquement pour ce copro
            vm.EvolutionSeries = new List<QuarterSeriesItem>();
            for (int q = 1; q <= 4; q++)
            {
                // Récupérer toutes les charges du trimestre q de l'année courante
                var charges = await _context.ChargeTrimestres
                    .Include(c => c.Trimestre)
                    .Where(c => c.Trimestre.Annee == currentYear && c.Trimestre.Numero == q)
                    .ToListAsync();

                decimal totalPrevisionnelForCopro = 0m;
                decimal totalReelForCopro = 0m;

                foreach (var ch in charges)
                {
                    // Allocation proportionnelle aux tantièmes du copro vs total du bâtiment
                    decimal proportion = 0m;
                    if (totalTantiemes > 0m)
                    {
                        proportion = (decimal)coproTantiemes / totalTantiemes;
                    }

                    totalPrevisionnelForCopro += ch.MontantPrevisionnel * proportion;
                    totalReelForCopro += ch.MontantReel * proportion;
                }

                vm.EvolutionSeries.Add(new QuarterSeriesItem
                {
                    Numero = q,
                    Previsionnel = decimal.Round(totalPrevisionnelForCopro, 2),
                    Reel = decimal.Round(totalReelForCopro, 2)
                });
            }

            // --- Repartition par sous-copro (donut) : pour les lots du copro, total des tantièmes par sous-copro
            var sousCoproGroup = vm.Lots
                .GroupBy(l => l.SousCopro ?? "Non renseigné")
                .Select(g => new DonutItem { Label = g.Key, Value = g.Sum(x => x.Tantiemes) })
                .ToList();

            vm.RepartitionSousCopro = sousCoproGroup;

            return View(vm);
        }
    }
}
