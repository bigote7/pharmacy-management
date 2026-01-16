using System.Text.Json.Serialization;

namespace PharmacyManagement.Application.DTOs;

// ViewModels pour le module Caissier
public class VenteCaissierViewModel
{
    [JsonPropertyName("clientId")]
    public int? ClientId { get; set; }
    
    [JsonPropertyName("clientNom")]
    public string? ClientNom { get; set; }
    
    [JsonPropertyName("clientTelephone")]
    public string? ClientTelephone { get; set; }
    
    [JsonPropertyName("lignes")]
    public List<LigneVenteViewModel> Lignes { get; set; } = new();
    
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
    
    [JsonPropertyName("modePaiement")]
    public string ModePaiement { get; set; } = "Cash"; // Cash, Carte, Assurance, Differe
    
    [JsonPropertyName("montantRecu")]
    public decimal? MontantRecu { get; set; }
}

public class LigneVenteViewModel
{
    public int MedicamentId { get; set; }
    public string MedicamentNom { get; set; } = string.Empty;
    public string CodeBarre { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal SousTotal { get; set; }
    public int StockDisponible { get; set; }
    public bool RequiertPrescription { get; set; }
}

public class PaiementViewModel
{
    public decimal MontantTotal { get; set; }
    public decimal MontantTVA { get; set; }
    public decimal MontantHT { get; set; }
    public string ModePaiement { get; set; } = "Cash";
    public decimal? MontantRecu { get; set; }
    public decimal? MontantRendu { get; set; }
}

public class VenteCompleteViewModel
{
    public int VenteId { get; set; }
    public string NumeroFacture { get; set; } = string.Empty;
    public DateTime DateVente { get; set; }
    public decimal MontantTotal { get; set; }
    public decimal MontantTVA { get; set; }
    public string ModePaiement { get; set; } = string.Empty;
    public string? ClientNom { get; set; }
    public List<LigneVenteViewModel> Lignes { get; set; } = new();
}

public class HistoriqueVenteViewModel
{
    public int Id { get; set; }
    public string NumeroFacture { get; set; } = string.Empty;
    public DateTime DateVente { get; set; }
    public decimal MontantTotal { get; set; }
    public string? ClientNom { get; set; }
    public string Statut { get; set; } = "Normal";
    public int NombreArticles { get; set; }
}

public class DashboardCaissierViewModel
{
    public int TotalVentesAujourdhui { get; set; }
    public decimal ChiffreAffairesAujourdhui { get; set; }
    public decimal ChiffreAffairesMois { get; set; }
    public int NombreClientsAujourdhui { get; set; }
    public List<MedicamentPopulaireDto> MedicamentsPlusVendus { get; set; } = new();
    public List<AlerteStockFaibleDto> AlertesStockFaible { get; set; } = new();
    public List<VenteRecenteDto> VentesRecentes { get; set; } = new();
}

public class AlerteStockFaibleDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public int StockDisponible { get; set; }
    public int StockMinimum { get; set; }
    public string Categorie { get; set; } = string.Empty;
}

public class RechercheMedicamentDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string CodeBarre { get; set; } = string.Empty;
    public decimal PrixVente { get; set; }
    public int StockDisponible { get; set; }
    public bool RequiertPrescription { get; set; }
    public string Categorie { get; set; } = string.Empty;
    public string Forme { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
}

