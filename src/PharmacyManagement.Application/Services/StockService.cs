using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;
using PharmacyManagement.Infrastructure.Repositories;

namespace PharmacyManagement.Application.Services;

public class StockService : IStockService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public StockService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<IEnumerable<StockDto>> GetAllStocksAsync()
    {
        var stocks = await _context.Stocks
            .Include(s => s.Medicament)
            .ToListAsync();

        return stocks.Select(s => new StockDto
        {
            Id = s.Id,
            MedicamentId = s.MedicamentId,
            MedicamentNom = s.Medicament.Nom,
            Quantite = s.Quantite,
            DateEntree = s.DateEntree,
            DateExpiration = s.DateExpiration,
            NumeroLot = s.NumeroLot,
            Notes = s.Notes,
            EstExpire = s.EstExpire,
            ExpireBientot = s.ExpireBientot
        });
    }

    public async Task<IEnumerable<StockDto>> GetStocksByMedicamentIdAsync(int medicamentId)
    {
        var stocks = await _context.Stocks
            .Include(s => s.Medicament)
            .Where(s => s.MedicamentId == medicamentId)
            .ToListAsync();

        return stocks.Select(s => new StockDto
        {
            Id = s.Id,
            MedicamentId = s.MedicamentId,
            MedicamentNom = s.Medicament.Nom,
            Quantite = s.Quantite,
            DateEntree = s.DateEntree,
            DateExpiration = s.DateExpiration,
            NumeroLot = s.NumeroLot,
            Notes = s.Notes,
            EstExpire = s.EstExpire,
            ExpireBientot = s.ExpireBientot
        });
    }

    public async Task<IEnumerable<StockDto>> GetStocksExpiresAsync()
    {
        var stocks = await _context.Stocks
            .Include(s => s.Medicament)
            .Where(s => s.EstExpire)
            .ToListAsync();

        return stocks.Select(s => new StockDto
        {
            Id = s.Id,
            MedicamentId = s.MedicamentId,
            MedicamentNom = s.Medicament.Nom,
            Quantite = s.Quantite,
            DateEntree = s.DateEntree,
            DateExpiration = s.DateExpiration,
            NumeroLot = s.NumeroLot,
            Notes = s.Notes,
            EstExpire = s.EstExpire,
            ExpireBientot = s.ExpireBientot
        });
    }

    public async Task<IEnumerable<StockDto>> GetStocksExpireBientotAsync()
    {
        var stocks = await _context.Stocks
            .Include(s => s.Medicament)
            .Where(s => s.ExpireBientot)
            .ToListAsync();

        return stocks.Select(s => new StockDto
        {
            Id = s.Id,
            MedicamentId = s.MedicamentId,
            MedicamentNom = s.Medicament.Nom,
            Quantite = s.Quantite,
            DateEntree = s.DateEntree,
            DateExpiration = s.DateExpiration,
            NumeroLot = s.NumeroLot,
            Notes = s.Notes,
            EstExpire = s.EstExpire,
            ExpireBientot = s.ExpireBientot
        });
    }

    public async Task<StockDto> CreateStockAsync(CreerStockDto dto)
    {
        var stock = new Stock
        {
            MedicamentId = dto.MedicamentId,
            Quantite = dto.Quantite,
            DateExpiration = dto.DateExpiration,
            NumeroLot = dto.NumeroLot,
            Notes = dto.Notes
        };

        await _unitOfWork.Stocks.AddAsync(stock);
        await _unitOfWork.SaveChangesAsync();

        var stockCree = await _context.Stocks
            .Include(s => s.Medicament)
            .FirstOrDefaultAsync(s => s.Id == stock.Id);

        if (stockCree == null)
            throw new Exception("Erreur lors de la création du stock");

        return new StockDto
        {
            Id = stockCree.Id,
            MedicamentId = stockCree.MedicamentId,
            MedicamentNom = stockCree.Medicament.Nom,
            Quantite = stockCree.Quantite,
            DateEntree = stockCree.DateEntree,
            DateExpiration = stockCree.DateExpiration,
            NumeroLot = stockCree.NumeroLot,
            Notes = stockCree.Notes,
            EstExpire = stockCree.EstExpire,
            ExpireBientot = stockCree.ExpireBientot
        };
    }

    public async Task<StockDto> UpdateStockAsync(int id, ModifierStockDto dto)
    {
        var stock = await _unitOfWork.Stocks.GetByIdAsync(id);
        if (stock == null)
            throw new Exception("Stock non trouvé");

        stock.Quantite = dto.Quantite;
        stock.DateExpiration = dto.DateExpiration;
        stock.NumeroLot = dto.NumeroLot;
        stock.Notes = dto.Notes;

        _unitOfWork.Stocks.Update(stock);
        await _unitOfWork.SaveChangesAsync();

        var stockModifie = await _context.Stocks
            .Include(s => s.Medicament)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (stockModifie == null)
            throw new Exception("Erreur lors de la mise à jour");

        return new StockDto
        {
            Id = stockModifie.Id,
            MedicamentId = stockModifie.MedicamentId,
            MedicamentNom = stockModifie.Medicament.Nom,
            Quantite = stockModifie.Quantite,
            DateEntree = stockModifie.DateEntree,
            DateExpiration = stockModifie.DateExpiration,
            NumeroLot = stockModifie.NumeroLot,
            Notes = stockModifie.Notes,
            EstExpire = stockModifie.EstExpire,
            ExpireBientot = stockModifie.ExpireBientot
        };
    }

    public async Task<bool> DeleteStockAsync(int id)
    {
        var stock = await _unitOfWork.Stocks.GetByIdAsync(id);
        if (stock == null)
            return false;

        _unitOfWork.Stocks.Remove(stock);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}

