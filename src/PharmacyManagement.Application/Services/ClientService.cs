using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;
using PharmacyManagement.Infrastructure.Repositories;

namespace PharmacyManagement.Application.Services;

public class ClientService : IClientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public ClientService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
    {
        var clients = await _unitOfWork.Clients.GetAllAsync();
        return clients.Select(c => new ClientDto
        {
            Id = c.Id,
            Nom = c.Nom,
            Prenom = c.Prenom,
            NomComplet = c.NomComplet,
            Telephone = c.Telephone,
            Email = c.Email,
            DateNaissance = c.DateNaissance,
            Adresse = c.Adresse,
            DateCreation = c.DateCreation
        });
    }

    public async Task<ClientDto?> GetClientByIdAsync(int id)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(id);
        if (client == null) return null;

        return new ClientDto
        {
            Id = client.Id,
            Nom = client.Nom,
            Prenom = client.Prenom,
            NomComplet = client.NomComplet,
            Telephone = client.Telephone,
            Email = client.Email,
            DateNaissance = client.DateNaissance,
            Adresse = client.Adresse,
            DateCreation = client.DateCreation
        };
    }

    public async Task<IEnumerable<ClientDto>> SearchClientsAsync(string searchTerm)
    {
        var clients = await _context.Clients
            .Where(c => c.Nom.Contains(searchTerm) || 
                       c.Prenom.Contains(searchTerm) || 
                       c.Telephone.Contains(searchTerm))
            .ToListAsync();

        return clients.Select(c => new ClientDto
        {
            Id = c.Id,
            Nom = c.Nom,
            Prenom = c.Prenom,
            NomComplet = c.NomComplet,
            Telephone = c.Telephone,
            Email = c.Email,
            DateNaissance = c.DateNaissance,
            Adresse = c.Adresse,
            DateCreation = c.DateCreation
        });
    }

    public async Task<ClientDto> CreateClientAsync(CreerClientDto dto)
    {
        var client = new Client
        {
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            Telephone = dto.Telephone,
            Email = dto.Email,
            DateNaissance = dto.DateNaissance,
            Adresse = dto.Adresse
        };

        await _unitOfWork.Clients.AddAsync(client);
        await _unitOfWork.SaveChangesAsync();

        return await GetClientByIdAsync(client.Id) ?? throw new Exception("Erreur lors de la création du client");
    }

    public async Task<ClientDto> UpdateClientAsync(int id, ModifierClientDto dto)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(id);
        if (client == null)
            throw new Exception("Client non trouvé");

        client.Nom = dto.Nom;
        client.Prenom = dto.Prenom;
        client.Telephone = dto.Telephone;
        client.Email = dto.Email;
        client.DateNaissance = dto.DateNaissance;
        client.Adresse = dto.Adresse;

        _unitOfWork.Clients.Update(client);
        await _unitOfWork.SaveChangesAsync();

        return await GetClientByIdAsync(id) ?? throw new Exception("Erreur lors de la mise à jour");
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(id);
        if (client == null)
            return false;

        _unitOfWork.Clients.Remove(client);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}

