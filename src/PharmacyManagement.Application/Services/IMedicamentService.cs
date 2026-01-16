using PharmacyManagement.Application.DTOs;

namespace PharmacyManagement.Application.Services;

public interface IMedicamentService
{
    Task<IEnumerable<MedicamentDto>> GetAllMedicamentsAsync();
    Task<MedicamentDto?> GetMedicamentByIdAsync(int id);
    Task<MedicamentDto?> GetMedicamentByCodeBarreAsync(string codeBarre);
    Task<MedicamentDto> CreateMedicamentAsync(CreerMedicamentDto dto);
    Task<MedicamentDto> UpdateMedicamentAsync(int id, ModifierMedicamentDto dto);
    Task<bool> DeleteMedicamentAsync(int id);
    Task<IEnumerable<MedicamentDto>> GetMedicamentsStockFaibleAsync();
    Task<int> GetStockDisponibleAsync(int medicamentId);
}

