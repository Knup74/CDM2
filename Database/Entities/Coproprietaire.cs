namespace CDM.Database.Models;

public class Coproprietaire
{
    public int Id { get; set; }

    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Auth
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

    public string Role { get; set; } = "Coproprietaire"; // ou "Admin"
    public bool IsActive { get; set; } = true;

    // Relations existantes
    public List<Lot> Lots { get; set; } = new();
}
