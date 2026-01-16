using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;
using PharmacyManagement.Infrastructure.Repositories;

namespace PharmacyManagement.Application.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public PrescriptionService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<IEnumerable<PrescriptionDto>> GetAllPrescriptionsAsync()
    {
        var prescriptions = await _context.Prescriptions
            .Include(p => p.Client)
            .Include(p => p.PrescriptionDetails)
                .ThenInclude(pd => pd.Medicament)
            .OrderByDescending(p => p.DatePrescription)
            .ToListAsync();

        return prescriptions.Select(p => new PrescriptionDto
        {
            Id = p.Id,
            NumeroPrescription = p.NumeroPrescription,
            DatePrescription = p.DatePrescription,
            NomMedecin = p.NomMedecin,
            Notes = p.Notes,
            EstUtilisee = p.EstUtilisee,
            DateUtilisation = p.DateUtilisation,
            ClientId = p.ClientId,
            ClientNom = p.Client.NomComplet,
            Details = p.PrescriptionDetails.Select(pd => new PrescriptionDetailDto
            {
                Id = pd.Id,
                MedicamentId = pd.MedicamentId,
                MedicamentNom = pd.Medicament.Nom,
                Quantite = pd.Quantite,
                Posologie = pd.Posologie
            }).ToList()
        });
    }

    public async Task<PrescriptionDto?> GetPrescriptionByIdAsync(int id)
    {
        var prescription = await _context.Prescriptions
            .Include(p => p.Client)
            .Include(p => p.PrescriptionDetails)
                .ThenInclude(pd => pd.Medicament)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (prescription == null) return null;

        return new PrescriptionDto
        {
            Id = prescription.Id,
            NumeroPrescription = prescription.NumeroPrescription,
            DatePrescription = prescription.DatePrescription,
            NomMedecin = prescription.NomMedecin,
            Notes = prescription.Notes,
            EstUtilisee = prescription.EstUtilisee,
            DateUtilisation = prescription.DateUtilisation,
            ClientId = prescription.ClientId,
            ClientNom = prescription.Client.NomComplet,
            Details = prescription.PrescriptionDetails.Select(pd => new PrescriptionDetailDto
            {
                Id = pd.Id,
                MedicamentId = pd.MedicamentId,
                MedicamentNom = pd.Medicament.Nom,
                Quantite = pd.Quantite,
                Posologie = pd.Posologie
            }).ToList()
        };
    }

    public async Task<PrescriptionDto> CreatePrescriptionAsync(CreerPrescriptionDto dto)
    {
        var prescription = new Prescription
        {
            NumeroPrescription = dto.NumeroPrescription,
            DatePrescription = dto.DatePrescription,
            NomMedecin = dto.NomMedecin,
            Notes = dto.Notes,
            ClientId = dto.ClientId,
            EstUtilisee = false
        };

        await _unitOfWork.Prescriptions.AddAsync(prescription);
        await _unitOfWork.SaveChangesAsync();

        foreach (var detailDto in dto.Details)
        {
            var prescriptionDetail = new PrescriptionDetail
            {
                PrescriptionId = prescription.Id,
                MedicamentId = detailDto.MedicamentId,
                Quantite = detailDto.Quantite,
                Posologie = detailDto.Posologie
            };

            await _unitOfWork.PrescriptionDetails.AddAsync(prescriptionDetail);
        }

        await _unitOfWork.SaveChangesAsync();

        return await GetPrescriptionByIdAsync(prescription.Id) ?? throw new Exception("Erreur lors de la création de la prescription");
    }

    public async Task<bool> UtiliserPrescriptionAsync(int id)
    {
        var prescription = await _unitOfWork.Prescriptions.GetByIdAsync(id);
        if (prescription == null || prescription.EstUtilisee)
            return false;

        prescription.EstUtilisee = true;
        prescription.DateUtilisation = DateTime.Now;

        _unitOfWork.Prescriptions.Update(prescription);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}

