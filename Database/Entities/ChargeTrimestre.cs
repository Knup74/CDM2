namespace CDM.Database.Models;


public class ChargeTrimestre
{
public int Id { get; set; }
public string Libelle { get; set; } = string.Empty;


// Montants au niveau de la charge
public decimal MontantPrevisionnel { get; set; }
public decimal MontantReel { get; set; }


public int TrimestreId { get; set; }
public Trimestre Trimestre { get; set; } = null!;


public List<ChargeTrimestreSousCopro> SousCopros { get; set; } = new();
}