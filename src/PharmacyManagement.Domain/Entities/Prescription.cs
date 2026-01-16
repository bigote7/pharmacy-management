namespace PharmacyManagement.Domain.Entities;

public class Prescription
{
    public int Id { get; set; }
    public string NumeroPrescription { get; set; } = string.Empty;
    public DateTime DatePrescription { get; set; }
    public string NomMedecin { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool EstUtilisee { get; set; } = false;
    public DateTime? DateUtilisation { get; set; }
    
    // Relations
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    
    public ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();
}

