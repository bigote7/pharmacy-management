namespace PharmacyManagement.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public bool EstActif { get; set; } = true;
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime? DerniereConnexion { get; set; }
    
    // Relations
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}

