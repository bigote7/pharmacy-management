namespace PharmacyManagement.Domain.Entities;

public class Stock
{
    public int Id { get; set; }
    public int Quantite { get; set; }
    public DateTime DateEntree { get; set; } = DateTime.Now;
    public DateTime DateExpiration { get; set; }
    public string NumeroLot { get; set; } = string.Empty;
    public string? Notes { get; set; }
    
    // Relations
    public int MedicamentId { get; set; }
    public Medicament Medicament { get; set; } = null!;
    
    public bool EstExpire => DateExpiration < DateTime.Now;
    public bool ExpireBientot => DateExpiration <= DateTime.Now.AddDays(30) && DateExpiration >= DateTime.Now;
}

