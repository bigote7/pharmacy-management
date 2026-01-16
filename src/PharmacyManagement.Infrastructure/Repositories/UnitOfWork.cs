using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    
    private IRepository<Medicament>? _medicaments;
    private IRepository<Categorie>? _categories;
    private IRepository<Client>? _clients;
    private IRepository<Vente>? _ventes;
    private IRepository<VenteDetail>? _venteDetails;
    private IRepository<Stock>? _stocks;
    private IRepository<Commande>? _commandes;
    private IRepository<CommandeDetail>? _commandeDetails;
    private IRepository<Fournisseur>? _fournisseurs;
    private IRepository<Prescription>? _prescriptions;
    private IRepository<PrescriptionDetail>? _prescriptionDetails;
    private IRepository<User>? _users;
    private IRepository<Role>? _roles;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<Medicament> Medicaments =>
        _medicaments ??= new Repository<Medicament>(_context);

    public IRepository<Categorie> Categories =>
        _categories ??= new Repository<Categorie>(_context);

    public IRepository<Client> Clients =>
        _clients ??= new Repository<Client>(_context);

    public IRepository<Vente> Ventes =>
        _ventes ??= new Repository<Vente>(_context);

    public IRepository<VenteDetail> VenteDetails =>
        _venteDetails ??= new Repository<VenteDetail>(_context);

    public IRepository<Stock> Stocks =>
        _stocks ??= new Repository<Stock>(_context);

    public IRepository<Commande> Commandes =>
        _commandes ??= new Repository<Commande>(_context);

    public IRepository<CommandeDetail> CommandeDetails =>
        _commandeDetails ??= new Repository<CommandeDetail>(_context);

    public IRepository<Fournisseur> Fournisseurs =>
        _fournisseurs ??= new Repository<Fournisseur>(_context);

    public IRepository<Prescription> Prescriptions =>
        _prescriptions ??= new Repository<Prescription>(_context);

    public IRepository<PrescriptionDetail> PrescriptionDetails =>
        _prescriptionDetails ??= new Repository<PrescriptionDetail>(_context);

    public IRepository<User> Users =>
        _users ??= new Repository<User>(_context);

    public IRepository<Role> Roles =>
        _roles ??= new Repository<Role>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

