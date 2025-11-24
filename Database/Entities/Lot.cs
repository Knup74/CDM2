namespace CDM.Database.Models;


public class Lot
{
public int Id { get; set; }
public string NumeroLot { get; set; } = string.Empty;
public int Tantiemes { get; set; }


public int CoproprietaireId { get; set; }
public Coproprietaire Coproprietaire { get; set; } = null!;


public int SousCoproprieteId { get; set; }
public SousCopropriete SousCopropriete { get; set; } = null!;
}