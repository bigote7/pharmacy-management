namespace PharmacyManagement.Domain.Entities;

public class Fournisseur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Adresse { get; set; }
    public bool EstActif { get; set; } = true;
    
    public ICollection<Commande> Commandes { get; set; } = new List<Commande>();
}

