using PharmacyManagement.Application.DTOs;

namespace PharmacyManagement.Application.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardDataAsync();
}

