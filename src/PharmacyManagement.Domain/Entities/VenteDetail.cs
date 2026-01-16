namespace PharmacyManagement.Domain.Entities;

public class VenteDetail
{
    public int Id { get; set; }
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal SousTotal { get; set; }
    
    // Relations
    public int VenteId { get; set; }
    public Vente Vente { get; set; } = null!;
    
    public int MedicamentId { get; set; }
    public Medicament Medicament { get; set; } = null!;
}

