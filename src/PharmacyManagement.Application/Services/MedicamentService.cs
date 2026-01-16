using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;
using PharmacyManagement.Infrastructure.Repositories;

namespace PharmacyManagement.Application.Services;

public class MedicamentService : IMedicamentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public MedicamentService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<IEnumerable<MedicamentDto>> GetAllMedicamentsAsync()
    {
        var medicaments = await _context.Medicaments
            .Include(m => m.Categorie)
            .Include(m => m.Stocks)
            .Where(m => m.EstActif)
            .ToListAsync();

        return medicaments.Select(m => new MedicamentDto
        {
            Id = m.Id,
            Nom = m.Nom,
            CodeBarre = m.CodeBarre,
            Dosage = m.Dosage,
            Forme = m.Forme,
            PrixAchat = m.PrixAchat,
            PrixVente = m.PrixVente,
            StockMinimum = m.StockMinimum,
            RequiertPrescription = m.RequiertPrescription,
            EstActif = m.EstActif,
            CategorieNom = m.Categorie.Nom,
            StockDisponible = m.Stocks.Where(s => !s.EstExpire).Sum(s => s.Quantite)
        });
    }

    public async Task<MedicamentDto?> GetMedicamentByIdAsync(int id)
    {
        var medicament = await _context.Medicaments
            .Include(m => m.Categorie)
            .Include(m => m.Stocks)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (medicament == null) return null;

        return new MedicamentDto
        {
            Id = medicament.Id,
            Nom = medicament.Nom,
            CodeBarre = medicament.CodeBarre,
            Dosage = medicament.Dosage,
            Forme = medicament.Forme,
            PrixAchat = medicament.PrixAchat,
            PrixVente = medicament.PrixVente,
            StockMinimum = medicament.StockMinimum,
            RequiertPrescription = medicament.RequiertPrescription,
            EstActif = medicament.EstActif,
            CategorieNom = medicament.Categorie.Nom,
            StockDisponible = medicament.Stocks.Where(s => !s.EstExpire).Sum(s => s.Quantite)
        };
    }

    public async Task<MedicamentDto?> GetMedicamentByCodeBarreAsync(string codeBarre)
    {
        var medicament = await _context.Medicaments
            .Include(m => m.Categorie)
            .Include(m => m.Stocks)
            .FirstOrDefaultAsync(m => m.CodeBarre == codeBarre);

        if (medicament == null) return null;

        return new MedicamentDto
        {
            Id = medicament.Id,
            Nom = medicament.Nom,
            CodeBarre = medicament.CodeBarre,
            Dosage = medicament.Dosage,
            Forme = medicament.Forme,
            PrixAchat = medicament.PrixAchat,
            PrixVente = medicament.PrixVente,
            StockMinimum = medicament.StockMinimum,
            RequiertPrescription = medicament.RequiertPrescription,
            EstActif = medicament.EstActif,
            CategorieNom = medicament.Categorie.Nom,
            StockDisponible = medicament.Stocks.Where(s => !s.EstExpire).Sum(s => s.Quantite)
        };
    }

    public async Task<MedicamentDto> CreateMedicamentAsync(CreerMedicamentDto dto)
    {
        var medicament = new Medicament
        {
            Nom = dto.Nom,
            CodeBarre = dto.CodeBarre,
            Dosage = dto.Dosage,
            Forme = dto.Forme,
            PrixAchat = dto.PrixAchat,
            PrixVente = dto.PrixVente,
            StockMinimum = dto.StockMinimum,
            RequiertPrescription = dto.RequiertPrescription,
            CategorieId = dto.CategorieId,
            EstActif = true
        };

        await _unitOfWork.Medicaments.AddAsync(medicament);
        await _unitOfWork.SaveChangesAsync();

        return await GetMedicamentByIdAsync(medicament.Id) ?? throw new Exception("Erreur lors de la création du médicament");
    }

    public async Task<MedicamentDto> UpdateMedicamentAsync(int id, ModifierMedicamentDto dto)
    {
        var medicament = await _unitOfWork.Medicaments.GetByIdAsync(id);
        if (medicament == null)
            throw new Exception("Médicament non trouvé");

        medicament.Nom = dto.Nom;
        medicament.CodeBarre = dto.CodeBarre;
        medicament.Dosage = dto.Dosage;
        medicament.Forme = dto.Forme;
        medicament.PrixAchat = dto.PrixAchat;
        medicament.PrixVente = dto.PrixVente;
        medicament.StockMinimum = dto.StockMinimum;
        medicament.RequiertPrescription = dto.RequiertPrescription;
        medicament.EstActif = dto.EstActif;
        medicament.CategorieId = dto.CategorieId;

        _unitOfWork.Medicaments.Update(medicament);
        await _unitOfWork.SaveChangesAsync();

        return await GetMedicamentByIdAsync(id) ?? throw new Exception("Erreur lors de la mise à jour");
    }

    public async Task<bool> DeleteMedicamentAsync(int id)
    {
        var medicament = await _unitOfWork.Medicaments.GetByIdAsync(id);
        if (medicament == null)
            return false;

        medicament.EstActif = false;
        _unitOfWork.Medicaments.Update(medicament);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<MedicamentDto>> GetMedicamentsStockFaibleAsync()
    {
        var medicaments = await GetAllMedicamentsAsync();
        return medicaments.Where(m => m.StockDisponible <= m.StockMinimum);
    }

    public async Task<int> GetStockDisponibleAsync(int medicamentId)
    {
        var medicament = await _context.Medicaments
            .Include(m => m.Stocks)
            .FirstOrDefaultAsync(m => m.Id == medicamentId);

        if (medicament == null) return 0;

        return medicament.Stocks.Where(s => !s.EstExpire).Sum(s => s.Quantite);
    }
}

