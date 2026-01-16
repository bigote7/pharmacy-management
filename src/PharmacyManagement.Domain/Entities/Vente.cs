namespace PharmacyManagement.Domain.Entities;

public class Vente
{
    public int Id { get; set; }
    public DateTime DateVente { get; set; } = DateTime.Now;
    public decimal MontantTotal { get; set; }
    public decimal MontantTVA { get; set; }
    public string NumeroFacture { get; set; } = string.Empty;
    public string? Notes { get; set; }
    
    // Gestion des retours/annulations
    public string Statut { get; set; } = "Normal"; // Normal, Annulee, Retournee
    public DateTime? DateAnnulation { get; set; }
    public string? RaisonAnnulation { get; set; }
    public int? UserId { get; set; } // Utilisateur qui a effectué la vente
    
    // Relations
    public int? ClientId { get; set; }
    public Client? Client { get; set; }
    public User? User { get; set; }
    
    public ICollection<VenteDetail> VenteDetails { get; set; } = new List<VenteDetail>();
}

