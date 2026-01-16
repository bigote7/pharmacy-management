using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;
using System.Globalization;
using PharmacyManagement.Application.Services;

namespace PharmacyManagement.Application.Services;

public class CaissierService : ICaissierService
{
    private readonly ApplicationDbContext _context;
    private readonly IVenteService _venteService;
    private const decimal TAUX_TVA = 0.20m; // 20% TVA

    public CaissierService(ApplicationDbContext context, IVenteService venteService)
    {
        _context = context;
        _venteService = venteService;
    }

    public async Task<DashboardCaissierViewModel> GetDashboardAsync(int userId)
    {
        var aujourdhui = DateTime.Now.Date;
        var demain = aujourdhui.AddDays(1);
        var debutMois = new DateTime(aujourdhui.Year, aujourdhui.Month, 1);

        var dashboard = new DashboardCaissierViewModel();

        // Ventes d'aujourd'hui
        try
        {
            dashboard.TotalVentesAujourdhui = await _context.Ventes
                .CountAsync(v => v.UserId == userId && 
                    v.DateVente >= aujourdhui && 
                    v.DateVente < demain && 
                    (v.Statut == null || v.Statut == "Normal" || v.Statut == ""));

            dashboard.ChiffreAffairesAujourdhui = await _context.Ventes
                .Where(v => v.UserId == userId && 
                    v.DateVente >= aujourdhui && 
                    v.DateVente < demain && 
                    (v.Statut == null || v.Statut == "Normal" || v.Statut == ""))
                .SumAsync(v => (decimal?)v.MontantTotal) ?? 0;
        }
        catch
        {
            dashboard.TotalVentesAujourdhui = await _context.Ventes
                .CountAsync(v => v.UserId == userId && v.DateVente >= aujourdhui && v.DateVente < demain);
            dashboard.ChiffreAffairesAujourdhui = await _context.Ventes
                .Where(v => v.UserId == userId && v.DateVente >= aujourdhui && v.DateVente < demain)
                .SumAsync(v => (decimal?)v.MontantTotal) ?? 0;
        }

        // Chiffre d'affaires du mois
        dashboard.ChiffreAffairesMois = await _context.Ventes
            .Where(v => v.UserId == userId && v.DateVente >= debutMois)
            .SumAsync(v => (decimal?)v.MontantTotal) ?? 0;

        // Clients servis aujourd'hui
        dashboard.NombreClientsAujourdhui = await _context.Ventes
            .Where(v => v.UserId == userId && v.DateVente >= aujourdhui && v.DateVente < demain && v.ClientId != null)
            .Select(v => v.ClientId)
            .Distinct()
            .CountAsync();

        // Médicaments les plus vendus
        var medicamentsPopulaires = await _context.VenteDetails
            .Include(vd => vd.Vente)
            .Include(vd => vd.Medicament)
            .Where(vd => vd.Vente.UserId == userId && vd.Vente.DateVente >= debutMois)
            .GroupBy(vd => new { vd.MedicamentId, vd.Medicament.Nom })
            .Select(g => new MedicamentPopulaireDto
            {
                MedicamentId = g.Key.MedicamentId,
                Nom = g.Key.Nom ?? "Inconnu",
                QuantiteVendue = g.Sum(vd => (int?)vd.Quantite) ?? 0,
                ChiffreAffaires = g.Sum(vd => (decimal?)vd.SousTotal) ?? 0
            })
            .OrderByDescending(m => m.QuantiteVendue)
            .Take(5)
            .ToListAsync();

        dashboard.MedicamentsPlusVendus = medicamentsPopulaires;

        // Alertes stock faible
        var medicaments = await _context.Medicaments
            .Include(m => m.Stocks)
            .Include(m => m.Categorie)
            .Where(m => m.EstActif)
            .ToListAsync();

        dashboard.AlertesStockFaible = medicaments
            .Where(m =>
            {
                var stockDisponible = m.Stocks.Where(s => !s.EstExpire).Sum(s => s.Quantite);
                return stockDisponible <= m.StockMinimum;
            })
            .Select(m => new AlerteStockFaibleDto
            {
                Id = m.Id,
                Nom = m.Nom,
                StockDisponible = m.Stocks.Where(s => !s.EstExpire).Sum(s => s.Quantite),
                StockMinimum = m.StockMinimum,
                Categorie = m.Categorie?.Nom ?? "Non catégorisé"
            })
            .ToList();

        // Ventes récentes
        var ventesRecentes = await _context.Ventes
            .Include(v => v.Client)
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.DateVente)
            .Take(10)
            .Select(v => new VenteRecenteDto
            {
                Id = v.Id,
                DateVente = v.DateVente,
                MontantTotal = v.MontantTotal,
                NumeroFacture = v.NumeroFacture,
                ClientNom = v.Client != null ? $"{v.Client.Nom} {v.Client.Prenom}" : null
            })
            .ToListAsync();

        dashboard.VentesRecentes = ventesRecentes;

        return dashboard;
    }

    public async Task<IEnumerable<RechercheMedicamentDto>> GetAllMedicamentsAsync()
    {
        var medicaments = await _context.Medicaments
            .Include(m => m.Categorie)
            .Include(m => m.Stocks)
            .Where(m => m.EstActif)
            .OrderBy(m => m.Nom)
            .ToListAsync();

        return medicaments.Select(medicament =>
        {
            // Utiliser la propriété EstExpire pour être cohérent avec les autres services
            var stockDisponible = medicament.Stocks
                .Where(s => !s.EstExpire)
                .Sum(s => s.Quantite);

            return new RechercheMedicamentDto
            {
                Id = medicament.Id,
                Nom = medicament.Nom,
                CodeBarre = medicament.CodeBarre,
                PrixVente = medicament.PrixVente,
                StockDisponible = stockDisponible,
                RequiertPrescription = medicament.RequiertPrescription,
                Categorie = medicament.Categorie?.Nom ?? "Non catégorisé",
                Forme = medicament.Forme,
                Dosage = medicament.Dosage
            };
        });
    }

    public async Task<IEnumerable<RechercheMedicamentDto>> RechercherMedicamentsAsync(string terme)
    {
        var medicaments = await _context.Medicaments
            .Include(m => m.Categorie)
            .Include(m => m.Stocks)
            .Where(m => m.EstActif && (
                m.Nom.Contains(terme) || 
                m.CodeBarre.Contains(terme) ||
                (m.Categorie != null && m.Categorie.Nom.Contains(terme))
            ))
            .Take(20) // Limiter à 20 résultats
            .ToListAsync();

        return medicaments.Select(medicament =>
        {
            // Utiliser la propriété EstExpire pour être cohérent avec les autres services
            var stockDisponible = medicament.Stocks
                .Where(s => !s.EstExpire)
                .Sum(s => s.Quantite);

            return new RechercheMedicamentDto
            {
                Id = medicament.Id,
                Nom = medicament.Nom,
                CodeBarre = medicament.CodeBarre,
                PrixVente = medicament.PrixVente,
                StockDisponible = stockDisponible,
                RequiertPrescription = medicament.RequiertPrescription,
                Categorie = medicament.Categorie?.Nom ?? "Non catégorisé",
                Forme = medicament.Forme,
                Dosage = medicament.Dosage
            };
        });
    }

    public async Task<RechercheMedicamentDto?> RechercherMedicamentParCodeBarreAsync(string codeBarre)
    {
        var medicament = await _context.Medicaments
            .Include(m => m.Categorie)
            .Include(m => m.Stocks)
            .Where(m => m.EstActif && m.CodeBarre == codeBarre)
            .FirstOrDefaultAsync();

        if (medicament == null) return null;

        // Utiliser la propriété EstExpire pour être cohérent avec les autres services
        var stockDisponible = medicament.Stocks
            .Where(s => !s.EstExpire)
            .Sum(s => s.Quantite);

        return new RechercheMedicamentDto
        {
            Id = medicament.Id,
            Nom = medicament.Nom,
            CodeBarre = medicament.CodeBarre,
            PrixVente = medicament.PrixVente,
            StockDisponible = stockDisponible,
            RequiertPrescription = medicament.RequiertPrescription,
            Categorie = medicament.Categorie?.Nom ?? "Non catégorisé",
            Forme = medicament.Forme,
            Dosage = medicament.Dosage
        };
    }

    public async Task<bool> ValiderStockAsync(int medicamentId, int quantite)
    {
        return await _venteService.ValiderStockDisponibleAsync(medicamentId, quantite);
    }

    public Task<decimal> CalculerTotalAvecTVAAsync(List<LigneVenteViewModel> lignes)
    {
        decimal montantHT = lignes.Sum(l => l.SousTotal);
        decimal montantTVA = montantHT * TAUX_TVA;
        return Task.FromResult(montantHT + montantTVA);
    }

    public async Task<VenteCompleteViewModel> CreerVenteAsync(VenteCaissierViewModel vente, int userId)
    {
        // Convertir en CreerVenteDto
        var creerVenteDto = new CreerVenteDto
        {
            ClientId = vente.ClientId,
            Notes = vente.Notes,
            Details = vente.Lignes.Select(l => new CreerVenteDetailDto
            {
                MedicamentId = l.MedicamentId,
                Quantite = l.Quantite
            }).ToList()
        };

        // Créer la vente via le service existant
        var venteCreee = await _venteService.CreateVenteAsync(creerVenteDto, userId);

        // Retourner le ViewModel complet
        return new VenteCompleteViewModel
        {
            VenteId = venteCreee.Id,
            NumeroFacture = venteCreee.NumeroFacture,
            DateVente = venteCreee.DateVente,
            MontantTotal = venteCreee.MontantTotal,
            MontantTVA = venteCreee.MontantTVA,
            ModePaiement = vente.ModePaiement,
            ClientNom = vente.ClientNom,
            Lignes = venteCreee.Details.Select(d => new LigneVenteViewModel
            {
                MedicamentId = d.MedicamentId,
                MedicamentNom = d.MedicamentNom,
                CodeBarre = string.Empty,
                Quantite = d.Quantite,
                PrixUnitaire = d.PrixUnitaire,
                SousTotal = d.SousTotal,
                StockDisponible = 0,
                RequiertPrescription = false
            }).ToList()
        };
    }

    public async Task<IEnumerable<HistoriqueVenteViewModel>> GetHistoriqueVentesAsync(int userId, DateTime? dateDebut = null, DateTime? dateFin = null)
    {
        var query = _context.Ventes
            .Include(v => v.Client)
            .Include(v => v.VenteDetails)
            .Where(v => v.UserId == userId);

        if (dateDebut.HasValue)
            query = query.Where(v => v.DateVente >= dateDebut.Value);

        if (dateFin.HasValue)
            query = query.Where(v => v.DateVente <= dateFin.Value);

        var ventes = await query
            .OrderByDescending(v => v.DateVente)
            .ToListAsync();

        return ventes.Select(v => new HistoriqueVenteViewModel
        {
            Id = v.Id,
            NumeroFacture = v.NumeroFacture,
            DateVente = v.DateVente,
            MontantTotal = v.MontantTotal,
            ClientNom = v.Client != null ? $"{v.Client.Nom} {v.Client.Prenom}" : null,
            Statut = v.Statut ?? "Normal",
            NombreArticles = v.VenteDetails.Sum(vd => vd.Quantite)
        });
    }

    public async Task<VenteCompleteViewModel> GetVenteDetailsAsync(int venteId, int userId)
    {
        var vente = await _context.Ventes
            .Include(v => v.Client)
            .Include(v => v.VenteDetails)
                .ThenInclude(vd => vd.Medicament)
            .FirstOrDefaultAsync(v => v.Id == venteId && v.UserId == userId);

        if (vente == null)
            throw new Exception("Vente non trouvée");

        return new VenteCompleteViewModel
        {
            VenteId = vente.Id,
            NumeroFacture = vente.NumeroFacture,
            DateVente = vente.DateVente,
            MontantTotal = vente.MontantTotal,
            MontantTVA = vente.MontantTVA,
            ClientNom = vente.Client != null ? $"{vente.Client.Nom} {vente.Client.Prenom}" : null,
            Lignes = vente.VenteDetails.Select(vd => new LigneVenteViewModel
            {
                MedicamentId = vd.MedicamentId,
                MedicamentNom = vd.Medicament.Nom,
                CodeBarre = vd.Medicament.CodeBarre,
                Quantite = vd.Quantite,
                PrixUnitaire = vd.PrixUnitaire,
                SousTotal = vd.SousTotal,
                StockDisponible = 0,
                RequiertPrescription = vd.Medicament.RequiertPrescription
            }).ToList()
        };
    }

    public async Task<bool> AnnulerVenteAsync(int venteId, string raison, int userId)
    {
        var vente = await _context.Ventes
            .FirstOrDefaultAsync(v => v.Id == venteId && v.UserId == userId);

        if (vente == null)
            return false;

        return await _venteService.AnnulerVenteAsync(venteId, raison, userId);
    }

    public async Task<IEnumerable<ClientDto>> RechercherClientsAsync(string terme)
    {
        var clients = await _context.Clients
            .Where(c => c.Nom.Contains(terme) || 
                       c.Prenom.Contains(terme) || 
                       c.Telephone.Contains(terme) ||
                       (c.Email != null && c.Email.Contains(terme)))
            .Take(10)
            .ToListAsync();

        return clients.Select(c => new ClientDto
        {
            Id = c.Id,
            Nom = c.Nom,
            Prenom = c.Prenom,
            NomComplet = $"{c.Nom} {c.Prenom}",
            Telephone = c.Telephone,
            Email = c.Email,
            Adresse = c.Adresse,
            DateCreation = c.DateCreation
        });
    }

    public async Task<ClientDto?> GetClientAsync(int clientId)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null) return null;

        return new ClientDto
        {
            Id = client.Id,
            Nom = client.Nom,
            Prenom = client.Prenom,
            NomComplet = $"{client.Nom} {client.Prenom}",
            Telephone = client.Telephone,
            Email = client.Email,
            Adresse = client.Adresse,
            DateCreation = client.DateCreation
        };
    }

    public async Task<IEnumerable<AlerteStockFaibleDto>> GetAlertesStockFaibleAsync()
    {
        var medicaments = await _context.Medicaments
            .Include(m => m.Categorie)
            .Include(m => m.Stocks)
            .Where(m => m.EstActif)
            .ToListAsync();

        return medicaments
            .Where(m =>
            {
                // Utiliser la propriété EstExpire pour être cohérent avec les autres services
                var stockDisponible = m.Stocks.Where(s => !s.EstExpire).Sum(s => s.Quantite);
                return stockDisponible <= m.StockMinimum;
            })
            .Select(m => new AlerteStockFaibleDto
            {
                Id = m.Id,
                Nom = m.Nom,
                StockDisponible = m.Stocks.Where(s => !s.EstExpire).Sum(s => s.Quantite),
                StockMinimum = m.StockMinimum,
                Categorie = m.Categorie?.Nom ?? "Non catégorisé"
            });
    }

    public async Task<IEnumerable<StockDto>> GetStocksExpirantBientotAsync()
    {
        var dateLimite = DateTime.Now.AddDays(30);
        var stocks = await _context.Stocks
            .Include(s => s.Medicament)
            .Where(s => s.DateExpiration >= DateTime.Now && s.DateExpiration <= dateLimite)
            .ToListAsync();

        return stocks.Select(s => new StockDto
        {
            Id = s.Id,
            MedicamentId = s.MedicamentId,
            MedicamentNom = s.Medicament.Nom,
            Quantite = s.Quantite,
            DateExpiration = s.DateExpiration,
            NumeroLot = s.NumeroLot
        });
    }

    public async Task<byte[]> GenererFacturePDFAsync(int venteId)
    {
        // TODO: Implémenter la génération PDF avec une bibliothèque comme QuestPDF ou iTextSharp
        // Pour l'instant, retourner un tableau vide
        await Task.CompletedTask;
        return Array.Empty<byte>();
    }

    public async Task<string> GenererTicket80mmAsync(int venteId)
    {
        // TODO: Implémenter la génération de ticket thermique 80mm
        // Pour l'instant, retourner une chaîne vide
        await Task.CompletedTask;
        return string.Empty;
    }
}

