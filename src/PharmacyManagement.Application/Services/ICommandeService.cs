using PharmacyManagement.Application.DTOs;

namespace PharmacyManagement.Application.Services;

public interface ICommandeService
{
    Task<IEnumerable<CommandeDto>> GetAllCommandesAsync();
    Task<CommandeDto?> GetCommandeByIdAsync(int id);
    Task<CommandeDto> CreateCommandeAsync(CreerCommandeDto dto);
    Task<CommandeDto> RecevoirCommandeAsync(int id, RecevoirCommandeDto dto);
    Task<bool> AnnulerCommandeAsync(int id);
}

