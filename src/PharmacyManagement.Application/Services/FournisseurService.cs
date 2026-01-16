using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Repositories;

namespace PharmacyManagement.Application.Services;

public class FournisseurService : IFournisseurService
{
    private readonly IUnitOfWork _unitOfWork;

    public FournisseurService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<FournisseurDto>> GetAllFournisseursAsync()
    {
        var fournisseurs = await _unitOfWork.Fournisseurs.GetAllAsync();
        return fournisseurs.Select(f => new FournisseurDto
        {
            Id = f.Id,
            Nom = f.Nom,
            Telephone = f.Telephone,
            Email = f.Email,
            Adresse = f.Adresse,
            EstActif = f.EstActif
        });
    }

    public async Task<FournisseurDto?> GetFournisseurByIdAsync(int id)
    {
        var fournisseur = await _unitOfWork.Fournisseurs.GetByIdAsync(id);
        if (fournisseur == null) return null;

        return new FournisseurDto
        {
            Id = fournisseur.Id,
            Nom = fournisseur.Nom,
            Telephone = fournisseur.Telephone,
            Email = fournisseur.Email,
            Adresse = fournisseur.Adresse,
            EstActif = fournisseur.EstActif
        };
    }

    public async Task<FournisseurDto> CreateFournisseurAsync(CreerFournisseurDto dto)
    {
        var fournisseur = new Fournisseur
        {
            Nom = dto.Nom,
            Telephone = dto.Telephone,
            Email = dto.Email,
            Adresse = dto.Adresse,
            EstActif = true
        };

        await _unitOfWork.Fournisseurs.AddAsync(fournisseur);
        await _unitOfWork.SaveChangesAsync();

        return await GetFournisseurByIdAsync(fournisseur.Id) ?? throw new Exception("Erreur lors de la création du fournisseur");
    }

    public async Task<FournisseurDto> UpdateFournisseurAsync(int id, ModifierFournisseurDto dto)
    {
        var fournisseur = await _unitOfWork.Fournisseurs.GetByIdAsync(id);
        if (fournisseur == null)
            throw new Exception("Fournisseur non trouvé");

        fournisseur.Nom = dto.Nom;
        fournisseur.Telephone = dto.Telephone;
        fournisseur.Email = dto.Email;
        fournisseur.Adresse = dto.Adresse;
        fournisseur.EstActif = dto.EstActif;

        _unitOfWork.Fournisseurs.Update(fournisseur);
        await _unitOfWork.SaveChangesAsync();

        return await GetFournisseurByIdAsync(id) ?? throw new Exception("Erreur lors de la mise à jour");
    }

    public async Task<bool> DeleteFournisseurAsync(int id)
    {
        var fournisseur = await _unitOfWork.Fournisseurs.GetByIdAsync(id);
        if (fournisseur == null)
            return false;

        fournisseur.EstActif = false;
        _unitOfWork.Fournisseurs.Update(fournisseur);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}

