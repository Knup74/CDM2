namespace CDM.Database.Models;


public class AppelDeFond
{
public int Id { get; set; }


public int TrimestreId { get; set; }
public Trimestre Trimestre { get; set; } = null!;


public int CoproprietaireId { get; set; }
public Coproprietaire Coproprietaire { get; set; } = null!;


// Montants calculés
public decimal MontantDu { get; set; } // montant prévisionnel affecté
public decimal MontantRegle { get; set; } // ce que le proprio a payé
public decimal Regularisation { get; set; } // différence à appliquer
}