namespace PharmacyManagement.Application.DTOs;

public class CommandeDto
{
    public int Id { get; set; }
    public DateTime DateCommande { get; set; }
    public DateTime? DateReception { get; set; }
    public string Statut { get; set; } = string.Empty;
    public decimal MontantTotal { get; set; }
    public string? Notes { get; set; }
    public int FournisseurId { get; set; }
    public string FournisseurNom { get; set; } = string.Empty;
    public List<CommandeDetailDto> Details { get; set; } = new();
}

public class CommandeDetailDto
{
    public int Id { get; set; }
    public int MedicamentId { get; set; }
    public string MedicamentNom { get; set; } = string.Empty;
    public int QuantiteCommandee { get; set; }
    public int QuantiteRecue { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal SousTotal { get; set; }
}

public class CreerCommandeDto
{
    public int FournisseurId { get; set; }
    public string? Notes { get; set; }
    public List<CreerCommandeDetailDto> Details { get; set; } = new();
}

public class CreerCommandeDetailDto
{
    public int MedicamentId { get; set; }
    public int QuantiteCommandee { get; set; }
    public decimal PrixUnitaire { get; set; }
}

public class RecevoirCommandeDto
{
    public List<RecevoirCommandeDetailDto> Details { get; set; } = new();
}

public class RecevoirCommandeDetailDto
{
    public int CommandeDetailId { get; set; }
    public int QuantiteRecue { get; set; }
}

