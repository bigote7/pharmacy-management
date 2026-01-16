namespace PharmacyManagement.Application.DTOs;

public class ClientDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime? DateNaissance { get; set; }
    public string? Adresse { get; set; }
    public DateTime DateCreation { get; set; }
}

public class CreerClientDto
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime? DateNaissance { get; set; }
    public string? Adresse { get; set; }
}

public class ModifierClientDto
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime? DateNaissance { get; set; }
    public string? Adresse { get; set; }
}

