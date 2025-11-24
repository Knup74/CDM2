namespace CDM.Database.Models;


public class SousCopropriete
{
public int Id { get; set; }
public string Nom { get; set; } = string.Empty;


public List<Lot> Lots { get; set; } = new();
public List<ChargeTrimestreSousCopro> ChargeAssociations { get; set; } = new();
}