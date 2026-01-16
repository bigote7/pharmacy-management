namespace PharmacyManagement.Application.DTOs;

public class StockDto
{
    public int Id { get; set; }
    public int MedicamentId { get; set; }
    public string MedicamentNom { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public DateTime DateEntree { get; set; }
    public DateTime DateExpiration { get; set; }
    public string NumeroLot { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool EstExpire { get; set; }
    public bool ExpireBientot { get; set; }
}

public class CreerStockDto
{
    public int MedicamentId { get; set; }
    public int Quantite { get; set; }
    public DateTime DateExpiration { get; set; }
    public string NumeroLot { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class ModifierStockDto
{
    public int Quantite { get; set; }
    public DateTime DateExpiration { get; set; }
    public string NumeroLot { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

