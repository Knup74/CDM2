namespace CDM.Database.Models;


public class Coproprietaire
{
public int Id { get; set; }
public string Nom { get; set; } = string.Empty;
public string? Email { get; set; }


public List<Lot> Lots { get; set; } = new();
public List<AppelDeFond> AppelsDeFonds { get; set; } = new();
}