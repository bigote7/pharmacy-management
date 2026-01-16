using PharmacyManagement.Application.DTOs;

namespace PharmacyManagement.Application.Services;

public interface IStockService
{
    Task<IEnumerable<StockDto>> GetAllStocksAsync();
    Task<IEnumerable<StockDto>> GetStocksByMedicamentIdAsync(int medicamentId);
    Task<IEnumerable<StockDto>> GetStocksExpiresAsync();
    Task<IEnumerable<StockDto>> GetStocksExpireBientotAsync();
    Task<StockDto> CreateStockAsync(CreerStockDto dto);
    Task<StockDto> UpdateStockAsync(int id, ModifierStockDto dto);
    Task<bool> DeleteStockAsync(int id);
}

