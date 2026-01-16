namespace PharmacyManagement.Domain.Entities;

public class CommandeDetail
{
    public int Id { get; set; }
    public int QuantiteCommandee { get; set; }
    public int QuantiteRecue { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal SousTotal { get; set; }
    
    // Relations
    public int CommandeId { get; set; }
    public Commande Commande { get; set; } = null!;
    
    public int MedicamentId { get; set; }
    public Medicament Medicament { get; set; } = null!;
}

