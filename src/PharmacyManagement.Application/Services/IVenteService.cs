using PharmacyManagement.Application.DTOs;

namespace PharmacyManagement.Application.Services;

public interface IVenteService
{
    Task<IEnumerable<VenteDto>> GetAllVentesAsync();
    Task<IEnumerable<VenteDto>> GetVentesByUserIdAsync(int userId);
    Task<VenteDto?> GetVenteByIdAsync(int id);
    Task<VenteDto> CreateVenteAsync(CreerVenteDto dto, int? userId = null);
    Task<bool> AnnulerVenteAsync(int id, string raison, int? userId = null);
    Task<bool> RetournerVenteAsync(int id, RetournerVenteDto dto, int? userId = null);
    Task<string> GenerateNumeroFactureAsync();
    Task<decimal> CalculerTotalAsync(List<CreerVenteDetailDto> details);
    Task<bool> ValiderStockDisponibleAsync(int medicamentId, int quantite);
}

