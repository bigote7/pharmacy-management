namespace PharmacyManagement.Application.DTOs;

public class VenteDto
{
    public int Id { get; set; }
    public DateTime DateVente { get; set; }
    public decimal MontantTotal { get; set; }
    public decimal MontantTVA { get; set; }
    public string NumeroFacture { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int? ClientId { get; set; }
    public string? ClientNom { get; set; }
    public string Statut { get; set; } = "Normal";
    public DateTime? DateAnnulation { get; set; }
    public string? RaisonAnnulation { get; set; }
    public int? UserId { get; set; }
    public string? UserNom { get; set; }
    public List<VenteDetailDto> Details { get; set; } = new();
}

public class VenteDetailDto
{
    public int Id { get; set; }
    public int MedicamentId { get; set; }
    public string MedicamentNom { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal SousTotal { get; set; }
}

public class CreerVenteDto
{
    public int? ClientId { get; set; }
    public string? Notes { get; set; }
    public List<CreerVenteDetailDto> Details { get; set; } = new();
}

public class CreerVenteDetailDto
{
    public int MedicamentId { get; set; }
    public int Quantite { get; set; }
}

public class AnnulerVenteDto
{
    public string Raison { get; set; } = string.Empty;
}

public class RetournerVenteDto
{
    public string Raison { get; set; } = string.Empty;
    public List<RetourVenteDetailDto> Details { get; set; } = new();
}

public class RetourVenteDetailDto
{
    public int VenteDetailId { get; set; }
    public int Quantite { get; set; }
}

