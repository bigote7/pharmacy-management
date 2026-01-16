using PharmacyManagement.Domain.Entities;

namespace PharmacyManagement.Infrastructure.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<Medicament> Medicaments { get; }
    IRepository<Categorie> Categories { get; }
    IRepository<Client> Clients { get; }
    IRepository<Vente> Ventes { get; }
    IRepository<VenteDetail> VenteDetails { get; }
    IRepository<Stock> Stocks { get; }
    IRepository<Commande> Commandes { get; }
    IRepository<CommandeDetail> CommandeDetails { get; }
    IRepository<Fournisseur> Fournisseurs { get; }
    IRepository<Prescription> Prescriptions { get; }
    IRepository<PrescriptionDetail> PrescriptionDetails { get; }
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

