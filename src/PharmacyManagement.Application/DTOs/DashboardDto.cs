













namespace PharmacyManagement.Application.DTOs;

public class DashboardDto
{
    public int TotalMedicaments { get; set; }
    public int TotalClients { get; set; }
    public int TotalVentes { get; set; }
    public decimal ChiffreAffairesTotal { get; set; }
    public int MedicamentsStockFaible { get; set; }
    public int StocksExpires { get; set; }
    public int StocksExpireBientot { get; set; }
    public int CommandesEnAttente { get; set; }
    
    // Données pour le caissier
    public int TotalVentesAujourdhui { get; set; }
    public decimal ChiffreAffairesAujourdhui { get; set; }
    public List<MedicamentPopulaireDto> MedicamentsPlusVendus { get; set; } = new();
    public List<MedicamentStockFaibleDto> AlertesStockFaible { get; set; } = new();
    public List<VenteRecenteDto> VentesRecentes { get; set; } = new();
    
    public List<VenteMensuelleDto> VentesMensuelles { get; set; } = new();
    public List<MedicamentPopulaireDto> MedicamentsPopulaires { get; set; } = new();
}

public class VenteMensuelleDto
{
    public string Mois { get; set; } = string.Empty;
    public decimal Montant { get; set; }
    public int NombreVentes { get; set; }
}

public class MedicamentPopulaireDto
{
    public int MedicamentId { get; set; }
    public string Nom { get; set; } = string.Empty;
    public int QuantiteVendue { get; set; }
    public decimal ChiffreAffaires { get; set; }
}

public class MedicamentStockFaibleDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public int StockDisponible { get; set; }
    public int StockMinimum { get; set; }
}

public class VenteRecenteDto
{
    public int Id { get; set; }
    public DateTime DateVente { get; set; }
    public decimal MontantTotal { get; set; }
    public string NumeroFacture { get; set; } = string.Empty;
    public string? ClientNom { get; set; }
}

