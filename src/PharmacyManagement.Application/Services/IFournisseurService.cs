using PharmacyManagement.Application.DTOs;

namespace PharmacyManagement.Application.Services;

public interface IFournisseurService
{
    Task<IEnumerable<FournisseurDto>> GetAllFournisseursAsync();
    Task<FournisseurDto?> GetFournisseurByIdAsync(int id);
    Task<FournisseurDto> CreateFournisseurAsync(CreerFournisseurDto dto);
    Task<FournisseurDto> UpdateFournisseurAsync(int id, ModifierFournisseurDto dto);
    Task<bool> DeleteFournisseurAsync(int id);
}

