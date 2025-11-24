using CDM.Database;
using CDM.Database.Models;
using Microsoft.EntityFrameworkCore;

public class ChargeCalculator
{
    private readonly AppDbContext _db;

    public ChargeCalculator(AppDbContext db)
    {
        _db = db;
    }

    // Calcule la somme des tantiemes pour un ensemble de sous-copro (ou tous les lots si null)
    private async Task<int> TotalTantiemesAsync(IEnumerable<int>? sousCoproIds = null)
    {
        if (sousCoproIds == null) return await _db.Lots.SumAsync(l => l.Tantiemes);
        return await _db.Lots.Where(l => sousCoproIds.Contains(l.SousCoproprieteId)).SumAsync(l => l.Tantiemes);
    }

    // Retourne la liste des lots concernés (selon sous-copros ou tous)
    private IQueryable<Lot> LotsConcernes(IEnumerable<int>? sousCoproIds = null)
    {
        var q = _db.Lots.Include(l => l.Coproprietaire).AsQueryable();
        if (sousCoproIds != null) q = q.Where(l => sousCoproIds.Contains(l.SousCoproprieteId));
        return q;
    }

    // Génère/Met à jour les appels de fonds pour un trimestre à partir des charges
    public async Task<List<AppelDeFond>> GenererAppelsDeFondsPourTrimestreAsync(int trimestreId)
    {
        var trimestre = await _db.Trimestres
            .Include(t => t.Charges).ThenInclude(c => c.SousCopros).ThenInclude(x => x.SousCopropriete)
            .FirstOrDefaultAsync(t => t.Id == trimestreId);
        if (trimestre == null) throw new InvalidOperationException("Trimestre introuvable");

        // Calculer total prévisionnel par lot : initialiser à 0
        var dictMontantParLot = new Dictionary<int, decimal>(); // lotId -> montant

        foreach (var charge in trimestre.Charges)
        {
            // Si charge.SousCopros vide => concerne toute la copro
            var sousIds = charge.SousCopros.Select(s => s.SousCoproprieteId).ToList();
            if (sousIds.Count == 0) sousIds = null; // null = tous

            var totalTantiemes = await TotalTantiemesAsync(sousIds);
            if (totalTantiemes == 0) continue;

            var lots = await LotsConcernes(sousIds).ToListAsync();

            foreach (var lot in lots)
            {
                var montantLot = (decimal)lot.Tantiemes / totalTantiemes * charge.MontantPrevisionnel;
                if (!dictMontantParLot.ContainsKey(lot.Id)) dictMontantParLot[lot.Id] = 0m;
                dictMontantParLot[lot.Id] += Math.Round(montantLot, 2);
            }
        }

        // Créer ou mettre à jour AppelDeFond par coproprietaire (somme des lots appartenant au copro)
        var result = new List<AppelDeFond>();

        var coproIds = await _db.Coproprietaires.Select(c => c.Id).ToListAsync();
        foreach (var coproId in coproIds)
        {
            // somme des montants des lots du copro
            var lotsDuCopro = await _db.Lots.Where(l => l.CoproprietaireId == coproId).Select(l => l.Id).ToListAsync();
            var montantDu = lotsDuCopro.Sum(lid => dictMontantParLot.ContainsKey(lid) ? dictMontantParLot[lid] : 0m);

            // récupérer si appel existe
            var appel = await _db.AppelsDeFonds.FirstOrDefaultAsync(a => a.TrimestreId == trimestreId && a.CoproprietaireId == coproId);
            if (appel == null)
            {
                appel = new AppelDeFond
                {
                    TrimestreId = trimestreId,
                    CoproprietaireId = coproId,
                    MontantDu = montantDu,
                    MontantRegle = 0m,
                    Regularisation = 0m
                };
                _db.AppelsDeFonds.Add(appel);
            }
            else
            {
                appel.MontantDu = montantDu;
            }

            result.Add(appel);
        }

        await _db.SaveChangesAsync();
        return result;
    }

    // Quand on connait les montants réels des charges (par trimestre), on appelle cette méthode
    // pour calculer la régularisation et appliquer la différence au trimestre suivant (optionnel)
    public async Task CalculerRegularisationAsync(int trimestreId)
    {
        var trimestre = await _db.Trimestres
            .Include(t => t.Charges)
            .Include(t => t.AppelsDeFonds)
            .FirstOrDefaultAsync(t => t.Id == trimestreId);
        if (trimestre == null) throw new InvalidOperationException("Trimestre introuvable");

        // Calculer le total réel du trimestre (somme des charges MontantReel)
        var totalReel = trimestre.Charges.Sum(c => c.MontantReel);
        var totalPrevisionnel = trimestre.Charges.Sum(c => c.MontantPrevisionnel);
        var diff = totalReel - totalPrevisionnel; // >0 : il manque de l'argent (on doit régulariser en plus)

        if (diff == 0) return;

        // On répartit la différence sur le trimestre suivant en créant/modifiant une charge spéciale
        var nextTrimestre = await _db.Trimestres.FirstOrDefaultAsync(t => t.Annee == trimestre.Annee && t.Numero == trimestre.Numero + 1)
            ?? await _db.Trimestres.FirstOrDefaultAsync(t => t.Annee == trimestre.Annee + 1 && t.Numero == 1);

        if (nextTrimestre == null)
        {
            // Option: créer le trimestre automatiquement
            throw new InvalidOperationException("Trimestre suivant introuvable. Créez-le avant d'appliquer la régularisation.");
        }

        // Créer une charge "Régularisation" sur nextTrimestre et la répartir comme une charge générale
        var regCharge = new ChargeTrimestre
        {
            Libelle = $"Régularisation T{trimestre.Numero}/{trimestre.Annee}",
            MontantPrevisionnel = diff,
            MontantReel = 0m,
            TrimestreId = nextTrimestre.Id
        };
        _db.ChargeTrimestres.Add(regCharge);
        await _db.SaveChangesAsync();

        // Regénérer les appels de fonds pour le trimestre suivant
        await GenererAppelsDeFondsPourTrimestreAsync(nextTrimestre.Id);
    }
}
