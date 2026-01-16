namespace PharmacyManagement.Domain.Entities;

public class Commande
{
    public int Id { get; set; }
    public DateTime DateCommande { get; set; } = DateTime.Now;
    public DateTime? DateReception { get; set; }
    public string Statut { get; set; } = "En attente"; // En attente, Reçue, Annulée
    public decimal MontantTotal { get; set; }
    public string? Notes { get; set; }
    
    // Relations
    public int FournisseurId { get; set; }
    public Fournisseur Fournisseur { get; set; } = null!;
    
    public ICollection<CommandeDetail> CommandeDetails { get; set; } = new List<CommandeDetail>();
}

