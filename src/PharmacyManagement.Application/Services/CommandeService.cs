using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;
using PharmacyManagement.Infrastructure.Repositories;

namespace PharmacyManagement.Application.Services;

public class CommandeService : ICommandeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public CommandeService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<IEnumerable<CommandeDto>> GetAllCommandesAsync()
    {
        var commandes = await _context.Commandes
            .Include(c => c.Fournisseur)
            .Include(c => c.CommandeDetails)
                .ThenInclude(cd => cd.Medicament)
            .OrderByDescending(c => c.DateCommande)
            .ToListAsync();

        return commandes.Select(c => new CommandeDto
        {
            Id = c.Id,
            DateCommande = c.DateCommande,
            DateReception = c.DateReception,
            Statut = c.Statut,
            MontantTotal = c.MontantTotal,
            Notes = c.Notes,
            FournisseurId = c.FournisseurId,
            FournisseurNom = c.Fournisseur.Nom,
            Details = c.CommandeDetails.Select(cd => new CommandeDetailDto
            {
                Id = cd.Id,
                MedicamentId = cd.MedicamentId,
                MedicamentNom = cd.Medicament.Nom,
                QuantiteCommandee = cd.QuantiteCommandee,
                QuantiteRecue = cd.QuantiteRecue,
                PrixUnitaire = cd.PrixUnitaire,
                SousTotal = cd.SousTotal
            }).ToList()
        });
    }

    public async Task<CommandeDto?> GetCommandeByIdAsync(int id)
    {
        var commande = await _context.Commandes
            .Include(c => c.Fournisseur)
            .Include(c => c.CommandeDetails)
                .ThenInclude(cd => cd.Medicament)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (commande == null) return null;

        return new CommandeDto
        {
            Id = commande.Id,
            DateCommande = commande.DateCommande,
            DateReception = commande.DateReception,
            Statut = commande.Statut,
            MontantTotal = commande.MontantTotal,
            Notes = commande.Notes,
            FournisseurId = commande.FournisseurId,
            FournisseurNom = commande.Fournisseur.Nom,
            Details = commande.CommandeDetails.Select(cd => new CommandeDetailDto
            {
                Id = cd.Id,
                MedicamentId = cd.MedicamentId,
                MedicamentNom = cd.Medicament.Nom,
                QuantiteCommandee = cd.QuantiteCommandee,
                QuantiteRecue = cd.QuantiteRecue,
                PrixUnitaire = cd.PrixUnitaire,
                SousTotal = cd.SousTotal
            }).ToList()
        };
    }

    public async Task<CommandeDto> CreateCommandeAsync(CreerCommandeDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var commande = new Commande
            {
                DateCommande = DateTime.Now,
                Statut = "En attente",
                FournisseurId = dto.FournisseurId,
                Notes = dto.Notes,
                MontantTotal = 0
            };

            await _unitOfWork.Commandes.AddAsync(commande);
            await _unitOfWork.SaveChangesAsync();

            decimal montantTotal = 0;

            foreach (var detailDto in dto.Details)
            {
                var commandeDetail = new CommandeDetail
                {
                    CommandeId = commande.Id,
                    MedicamentId = detailDto.MedicamentId,
                    QuantiteCommandee = detailDto.QuantiteCommandee,
                    QuantiteRecue = 0,
                    PrixUnitaire = detailDto.PrixUnitaire,
                    SousTotal = detailDto.PrixUnitaire * detailDto.QuantiteCommandee
                };

                montantTotal += commandeDetail.SousTotal;
                await _unitOfWork.CommandeDetails.AddAsync(commandeDetail);
            }

            commande.MontantTotal = montantTotal;
            _unitOfWork.Commandes.Update(commande);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return await GetCommandeByIdAsync(commande.Id) ?? throw new Exception("Erreur lors de la création de la commande");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<CommandeDto> RecevoirCommandeAsync(int id, RecevoirCommandeDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var commande = await _context.Commandes
                .Include(c => c.CommandeDetails)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (commande == null)
                throw new Exception("Commande non trouvée");

            if (commande.Statut != "En attente")
                throw new Exception("Cette commande ne peut plus être modifiée");

            foreach (var detailDto in dto.Details)
            {
                var commandeDetail = commande.CommandeDetails.FirstOrDefault(cd => cd.Id == detailDto.CommandeDetailId);
                if (commandeDetail != null)
                {
                    commandeDetail.QuantiteRecue = detailDto.QuantiteRecue;

                    // Ajouter au stock
                    var stock = new Stock
                    {
                        MedicamentId = commandeDetail.MedicamentId,
                        Quantite = detailDto.QuantiteRecue,
                        DateEntree = DateTime.Now,
                        DateExpiration = DateTime.Now.AddYears(2), // Par défaut, ajustez selon vos besoins
                        NumeroLot = $"LOT-{DateTime.Now:yyyyMMdd}-{commandeDetail.Id}"
                    };

                    await _unitOfWork.Stocks.AddAsync(stock);
                }
            }

            commande.DateReception = DateTime.Now;
            commande.Statut = "Reçue";
            _unitOfWork.Commandes.Update(commande);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return await GetCommandeByIdAsync(id) ?? throw new Exception("Erreur lors de la réception de la commande");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> AnnulerCommandeAsync(int id)
    {
        var commande = await _unitOfWork.Commandes.GetByIdAsync(id);
        if (commande == null || commande.Statut != "En attente")
            return false;

        commande.Statut = "Annulée";
        _unitOfWork.Commandes.Update(commande);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}

