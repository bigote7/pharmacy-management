namespace PharmacyManagement.Domain.Entities;

public class Client
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime? DateNaissance { get; set; }
    public string? Adresse { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.Now;
    
    // Relations
    public ICollection<Vente> Ventes { get; set; } = new List<Vente>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    
    public string NomComplet => $"{Prenom} {Nom}";
}

