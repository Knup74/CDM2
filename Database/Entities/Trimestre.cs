namespace CDM.Database.Models;


public class Trimestre
{
public int Id { get; set; }
public int Annee { get; set; }
public int Numero { get; set; } // 1..4


public decimal TotalPrevisionnel { get; set; }
public decimal TotalReel { get; set; }


public bool EstValide { get; set; }


public List<ChargeTrimestre> Charges { get; set; } = new();
public List<AppelDeFond> AppelsDeFonds { get; set; } = new();
}