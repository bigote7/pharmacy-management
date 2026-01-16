namespace PharmacyManagement.Domain.Entities;

public class Categorie
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public ICollection<Medicament> Medicaments { get; set; } = new List<Medicament>();
}

