namespace PharmacyManagement.Domain.Entities;

public class PrescriptionDetail
{
    public int Id { get; set; }
    public int Quantite { get; set; }
    public string? Posologie { get; set; } // Instructions d'utilisation
    
    // Relations
    public int PrescriptionId { get; set; }
    public Prescription Prescription { get; set; } = null!;
    
    public int MedicamentId { get; set; }
    public Medicament Medicament { get; set; } = null!;
}

