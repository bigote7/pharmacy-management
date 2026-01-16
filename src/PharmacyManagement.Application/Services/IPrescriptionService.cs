using PharmacyManagement.Application.DTOs;

namespace PharmacyManagement.Application.Services;

public interface IPrescriptionService
{
    Task<IEnumerable<PrescriptionDto>> GetAllPrescriptionsAsync();
    Task<PrescriptionDto?> GetPrescriptionByIdAsync(int id);
    Task<PrescriptionDto> CreatePrescriptionAsync(CreerPrescriptionDto dto);
    Task<bool> UtiliserPrescriptionAsync(int id);
}

