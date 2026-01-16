namespace PharmacyManagement.Application.DTOs;

public class PrescriptionDto
{
    public int Id { get; set; }
    public string NumeroPrescription { get; set; } = string.Empty;
    public DateTime DatePrescription { get; set; }
    public string NomMedecin { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool EstUtilisee { get; set; }
    public DateTime? DateUtilisation { get; set; }
    public int ClientId { get; set; }
    public string ClientNom { get; set; } = string.Empty;
    public List<PrescriptionDetailDto> Details { get; set; } = new();
}

public class PrescriptionDetailDto
{
    public int Id { get; set; }
    public int MedicamentId { get; set; }
    public string MedicamentNom { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public string? Posologie { get; set; }
}

public class CreerPrescriptionDto
{
    public string NumeroPrescription { get; set; } = string.Empty;
    public DateTime DatePrescription { get; set; }
    public string NomMedecin { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int ClientId { get; set; }
    public List<CreerPrescriptionDetailDto> Details { get; set; } = new();
}

public class CreerPrescriptionDetailDto
{
    public int MedicamentId { get; set; }
    public int Quantite { get; set; }
    public string? Posologie { get; set; }
}

