using PharmacyManagement.Application.DTOs;

namespace PharmacyManagement.Application.Services;

public interface IClientService
{
    Task<IEnumerable<ClientDto>> GetAllClientsAsync();
    Task<ClientDto?> GetClientByIdAsync(int id);
    Task<IEnumerable<ClientDto>> SearchClientsAsync(string searchTerm);
    Task<ClientDto> CreateClientAsync(CreerClientDto dto);
    Task<ClientDto> UpdateClientAsync(int id, ModifierClientDto dto);
    Task<bool> DeleteClientAsync(int id);
}

