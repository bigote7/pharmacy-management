using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Domain.Entities;

namespace PharmacyManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Categorie> Categories { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Vente> Ventes { get; set; }
    public DbSet<VenteDetail> VenteDetails { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Commande> Commandes { get; set; }
    public DbSet<CommandeDetail> CommandeDetails { get; set; }
    public DbSet<Fournisseur> Fournisseurs { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionDetail> PrescriptionDetails { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Indexes
        modelBuilder.Entity<Medicament>()
            .HasIndex(m => m.CodeBarre)
            .IsUnique();
            
        modelBuilder.Entity<Client>()
            .HasIndex(c => c.Telephone);
            
        modelBuilder.Entity<Vente>()
            .HasIndex(v => v.NumeroFacture)
            .IsUnique();
            
        modelBuilder.Entity<Prescription>()
            .HasIndex(p => p.NumeroPrescription);
    }
}

