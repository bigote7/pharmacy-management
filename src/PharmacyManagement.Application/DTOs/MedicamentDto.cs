namespace PharmacyManagement.Application.DTOs;

public class MedicamentDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string CodeBarre { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Forme { get; set; } = string.Empty;
    public decimal PrixAchat { get; set; }
    public decimal PrixVente { get; set; }
    public int StockMinimum { get; set; }
    public bool RequiertPrescription { get; set; }
    public bool EstActif { get; set; }
    public string CategorieNom { get; set; } = string.Empty;
    public int StockDisponible { get; set; }
}

public class CreerMedicamentDto
{
    public string Nom { get; set; } = string.Empty;
    public string CodeBarre { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Forme { get; set; } = string.Empty;
    public decimal PrixAchat { get; set; }
    public decimal PrixVente { get; set; }
    public int StockMinimum { get; set; }
    public bool RequiertPrescription { get; set; }
    public int CategorieId { get; set; }
}

public class ModifierMedicamentDto
{
    public string Nom { get; set; } = string.Empty;
    public string CodeBarre { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Forme { get; set; } = string.Empty;
    public decimal PrixAchat { get; set; }
    public decimal PrixVente { get; set; }
    public int StockMinimum { get; set; }
    public bool RequiertPrescription { get; set; }
    public bool EstActif { get; set; }
    public int CategorieId { get; set; }
}

