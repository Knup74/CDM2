namespace CDM.Database.Models;


public class ChargeTrimestreSousCopro
{
public int ChargeTrimestreId { get; set; }
public ChargeTrimestre ChargeTrimestre { get; set; } = null!;


public int SousCoproprieteId { get; set; }
public SousCopropriete SousCopropriete { get; set; } = null!;
}