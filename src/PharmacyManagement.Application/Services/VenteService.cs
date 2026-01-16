using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;
using PharmacyManagement.Infrastructure.Repositories;

namespace PharmacyManagement.Application.Services;

public class VenteService : IVenteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;
    private const decimal TAUX_TVA = 0.20m; // 20% TVA

    public VenteService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<IEnumerable<VenteDto>> GetAllVentesAsync()
    {
        var ventes = await _context.Ventes
            .Include(v => v.Client)
            .Include(v => v.User)
            .Include(v => v.VenteDetails)
                .ThenInclude(vd => vd.Medicament)
            .OrderByDescending(v => v.DateVente)
            .ToListAsync();

        return ventes.Select(v => new VenteDto
        {
            Id = v.Id,
            DateVente = v.DateVente,
            MontantTotal = v.MontantTotal,
            MontantTVA = v.MontantTVA,
            NumeroFacture = v.NumeroFacture,
            Notes = v.Notes,
            ClientId = v.ClientId,
            ClientNom = v.Client?.NomComplet,
            Statut = v.Statut,
            DateAnnulation = v.DateAnnulation,
            RaisonAnnulation = v.RaisonAnnulation,
            UserId = v.UserId,
            UserNom = v.User?.FullName,
            Details = v.VenteDetails.Select(vd => new VenteDetailDto
            {
                Id = vd.Id,
                MedicamentId = vd.MedicamentId,
                MedicamentNom = vd.Medicament.Nom,
                Quantite = vd.Quantite,
                PrixUnitaire = vd.PrixUnitaire,
                SousTotal = vd.SousTotal
            }).ToList()
        });
    }

    public async Task<VenteDto?> GetVenteByIdAsync(int id)
    {
        var vente = await _context.Ventes
            .Include(v => v.Client)
            .Include(v => v.User)
            .Include(v => v.VenteDetails)
                .ThenInclude(vd => vd.Medicament)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vente == null) return null;

        return new VenteDto
        {
            Id = vente.Id,
            DateVente = vente.DateVente,
            MontantTotal = vente.MontantTotal,
            MontantTVA = vente.MontantTVA,
            NumeroFacture = vente.NumeroFacture,
            Notes = vente.Notes,
            ClientId = vente.ClientId,
            ClientNom = vente.Client?.NomComplet,
            Statut = vente.Statut,
            DateAnnulation = vente.DateAnnulation,
            RaisonAnnulation = vente.RaisonAnnulation,
            UserId = vente.UserId,
            UserNom = vente.User?.FullName,
            Details = vente.VenteDetails.Select(vd => new VenteDetailDto
            {
                Id = vd.Id,
                MedicamentId = vd.MedicamentId,
                MedicamentNom = vd.Medicament.Nom,
                Quantite = vd.Quantite,
                PrixUnitaire = vd.PrixUnitaire,
                SousTotal = vd.SousTotal
            }).ToList()
        };
    }

    public async Task<IEnumerable<VenteDto>> GetVentesByUserIdAsync(int userId)
    {
        var ventes = await _context.Ventes
            .Include(v => v.Client)
            .Include(v => v.User)
            .Include(v => v.VenteDetails)
                .ThenInclude(vd => vd.Medicament)
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.DateVente)
            .ToListAsync();

        return ventes.Select(v => new VenteDto
        {
            Id = v.Id,
            DateVente = v.DateVente,
            MontantTotal = v.MontantTotal,
            MontantTVA = v.MontantTVA,
            NumeroFacture = v.NumeroFacture,
            Notes = v.Notes,
            ClientId = v.ClientId,
            ClientNom = v.Client?.NomComplet,
            Statut = v.Statut,
            DateAnnulation = v.DateAnnulation,
            RaisonAnnulation = v.RaisonAnnulation,
            UserId = v.UserId,
            UserNom = v.User?.FullName,
            Details = v.VenteDetails.Select(vd => new VenteDetailDto
            {
                Id = vd.Id,
                MedicamentId = vd.MedicamentId,
                MedicamentNom = vd.Medicament.Nom,
                Quantite = vd.Quantite,
                PrixUnitaire = vd.PrixUnitaire,
                SousTotal = vd.SousTotal
            }).ToList()
        });
    }

    public async Task<VenteDto> CreateVenteAsync(CreerVenteDto dto, int? userId = null)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Valider les stocks
            foreach (var detail in dto.Details)
            {
                if (!await ValiderStockDisponibleAsync(detail.MedicamentId, detail.Quantite))
                {
                    throw new Exception($"Stock insuffisant pour le médicament ID: {detail.MedicamentId}");
                }
            }

            // Générer le numéro de facture
            var numeroFacture = await GenerateNumeroFactureAsync();

            // Calculer le total
            var montantHT = await CalculerTotalAsync(dto.Details);
            var montantTVA = montantHT * TAUX_TVA;
            var montantTotal = montantHT + montantTVA;

            // Créer la vente
            var vente = new Vente
            {
                DateVente = DateTime.Now,
                MontantTotal = montantTotal,
                MontantTVA = montantTVA,
                NumeroFacture = numeroFacture,
                Notes = dto.Notes,
                ClientId = dto.ClientId,
                UserId = userId,
                Statut = "Normal"
            };

            await _unitOfWork.Ventes.AddAsync(vente);
            await _unitOfWork.SaveChangesAsync();

            // Créer les détails et mettre à jour les stocks
            foreach (var detailDto in dto.Details)
            {
                var medicament = await _context.Medicaments
                    .Include(m => m.Stocks)
                    .FirstOrDefaultAsync(m => m.Id == detailDto.MedicamentId);

                if (medicament == null)
                    throw new Exception($"Médicament non trouvé: {detailDto.MedicamentId}");

                var venteDetail = new VenteDetail
                {
                    VenteId = vente.Id,
                    MedicamentId = detailDto.MedicamentId,
                    Quantite = detailDto.Quantite,
                    PrixUnitaire = medicament.PrixVente,
                    SousTotal = medicament.PrixVente * detailDto.Quantite
                };

                await _unitOfWork.VenteDetails.AddAsync(venteDetail);

                // Déduire du stock (FIFO - First In First Out)
                await DeduireStockAsync(medicament, detailDto.Quantite);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return await GetVenteByIdAsync(vente.Id) ?? throw new Exception("Erreur lors de la création de la vente");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private Task DeduireStockAsync(Medicament medicament, int quantite)
    {
        // Utiliser la propriété EstExpire pour être cohérent avec les autres services
        var stocksDisponibles = medicament.Stocks
            .Where(s => !s.EstExpire && s.Quantite > 0)
            .OrderBy(s => s.DateEntree) // FIFO - First In First Out
            .ToList();

        int quantiteRestante = quantite;

        foreach (var stock in stocksDisponibles)
        {
            if (quantiteRestante <= 0) break;

            if (stock.Quantite >= quantiteRestante)
            {
                stock.Quantite -= quantiteRestante;
                quantiteRestante = 0;
            }
            else
            {
                quantiteRestante -= stock.Quantite;
                stock.Quantite = 0;
            }

            _unitOfWork.Stocks.Update(stock);
        }

        if (quantiteRestante > 0)
        {
            throw new Exception($"Stock insuffisant pour {medicament.Nom}");
        }

        return Task.CompletedTask;
    }

    public async Task<string> GenerateNumeroFactureAsync()
    {
        var date = DateTime.Now;
        var prefix = $"FAC-{date:yyyyMMdd}-";
        
        var dernierNumero = await _context.Ventes
            .Where(v => v.NumeroFacture.StartsWith(prefix))
            .OrderByDescending(v => v.NumeroFacture)
            .Select(v => v.NumeroFacture)
            .FirstOrDefaultAsync();

        int numero = 1;
        if (dernierNumero != null)
        {
            var numeroStr = dernierNumero.Replace(prefix, "");
            if (int.TryParse(numeroStr, out int num))
            {
                numero = num + 1;
            }
        }

        return $"{prefix}{numero:D4}";
    }

    public async Task<decimal> CalculerTotalAsync(List<CreerVenteDetailDto> details)
    {
        decimal total = 0;

        foreach (var detail in details)
        {
            var medicament = await _unitOfWork.Medicaments.GetByIdAsync(detail.MedicamentId);
            if (medicament == null)
                throw new Exception($"Médicament non trouvé: {detail.MedicamentId}");

            total += medicament.PrixVente * detail.Quantite;
        }

        return total;
    }

    public async Task<bool> ValiderStockDisponibleAsync(int medicamentId, int quantite)
    {
        var medicament = await _context.Medicaments
            .Include(m => m.Stocks)
            .FirstOrDefaultAsync(m => m.Id == medicamentId);

        if (medicament == null) return false;

        // Utiliser la propriété EstExpire pour être cohérent avec les autres services
        var stockDisponible = medicament.Stocks
            .Where(s => !s.EstExpire)
            .Sum(s => s.Quantite);

        return stockDisponible >= quantite;
    }

    public async Task<bool> AnnulerVenteAsync(int id, string raison, int? userId = null)
    {
        var vente = await _context.Ventes
            .Include(v => v.VenteDetails)
                .ThenInclude(vd => vd.Medicament)
                    .ThenInclude(m => m.Stocks)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vente == null || vente.Statut != "Normal")
            return false;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Restaurer les stocks
            foreach (var detail in vente.VenteDetails)
            {
                await RestaurerStockAsync(detail.Medicament, detail.Quantite);
            }

            // Marquer la vente comme annulée
            vente.Statut = "Annulee";
            vente.DateAnnulation = DateTime.Now;
            vente.RaisonAnnulation = raison;

            _unitOfWork.Ventes.Update(vente);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> RetournerVenteAsync(int id, RetournerVenteDto dto, int? userId = null)
    {
        var vente = await _context.Ventes
            .Include(v => v.VenteDetails)
                .ThenInclude(vd => vd.Medicament)
                    .ThenInclude(m => m.Stocks)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vente == null || vente.Statut != "Normal")
            return false;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Restaurer les stocks pour les articles retournés
            foreach (var retourDetail in dto.Details)
            {
                var venteDetail = vente.VenteDetails.FirstOrDefault(vd => vd.Id == retourDetail.VenteDetailId);
                if (venteDetail != null && retourDetail.Quantite > 0 && retourDetail.Quantite <= venteDetail.Quantite)
                {
                    await RestaurerStockAsync(venteDetail.Medicament, retourDetail.Quantite);
                }
            }

            // Marquer la vente comme retournée
            vente.Statut = "Retournee";
            vente.DateAnnulation = DateTime.Now;
            vente.RaisonAnnulation = dto.Raison;

            _unitOfWork.Ventes.Update(vente);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private async Task RestaurerStockAsync(Medicament medicament, int quantite)
    {
        // Trouver le stock le plus récent pour restaurer
        var stock = await _context.Stocks
            .Where(s => s.MedicamentId == medicament.Id && s.DateExpiration >= DateTime.Now)
            .OrderByDescending(s => s.DateEntree)
            .FirstOrDefaultAsync();

        if (stock != null)
        {
            stock.Quantite += quantite;
            _unitOfWork.Stocks.Update(stock);
        }
        else
        {
            // Créer un nouveau stock si aucun n'existe
            var nouveauStock = new Stock
            {
                MedicamentId = medicament.Id,
                Quantite = quantite,
                DateEntree = DateTime.Now,
                DateExpiration = DateTime.Now.AddYears(2),
                NumeroLot = $"RET-{DateTime.Now:yyyyMMddHHmmss}"
            };
            await _unitOfWork.Stocks.AddAsync(nouveauStock);
        }
    }
}

