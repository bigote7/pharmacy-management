namespace PharmacyManagement.Domain.Entities;

public class Medicament
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string CodeBarre { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Forme { get; set; } = string.Empty; // Comprimé, Sirop, Gélule, etc.
    public decimal PrixAchat { get; set; }
    public decimal PrixVente { get; set; }
    public int StockMinimum { get; set; }
    public bool RequiertPrescription { get; set; }
    public bool EstActif { get; set; } = true;
    
    // Relations
    public int CategorieId { get; set; }
    public Categorie Categorie { get; set; } = null!;
    
    public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    public ICollection<VenteDetail> VenteDetails { get; set; } = new List<VenteDetail>();
    public ICollection<CommandeDetail> CommandeDetails { get; set; } = new List<CommandeDetail>();
    public ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();
}

