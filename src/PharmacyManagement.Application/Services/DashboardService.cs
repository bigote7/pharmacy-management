using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        var dashboard = new DashboardDto();

        // Statistiques de base
        dashboard.TotalMedicaments = await _context.Medicaments.CountAsync(m => m.EstActif);
        dashboard.TotalClients = await _context.Clients.CountAsync();
        dashboard.TotalVentes = await _context.Ventes.CountAsync();
        dashboard.ChiffreAffairesTotal = await _context.Ventes.SumAsync(v => (decimal?)v.MontantTotal) ?? 0;

        // Médicaments en stock faible
        var medicaments = await _context.Medicaments
            .Include(m => m.Stocks)
            .Where(m => m.EstActif)
            .ToListAsync();

        dashboard.MedicamentsStockFaible = medicaments.Count(m =>
        {
            var stockDisponible = m.Stocks.Where(s => s.DateExpiration >= DateTime.Now).Sum(s => s.Quantite);
            return stockDisponible <= m.StockMinimum;
        });

        // Stocks expirés et expirant bientôt
        var stocks = await _context.Stocks.ToListAsync();
        dashboard.StocksExpires = stocks.Count(s => s.EstExpire);
        dashboard.StocksExpireBientot = stocks.Count(s => s.ExpireBientot);

        // Commandes en attente
        dashboard.CommandesEnAttente = await _context.Commandes
            .CountAsync(c => c.Statut == "En attente");

        // Ventes mensuelles (6 derniers mois)
        var sixMoisAuparavant = DateTime.Now.AddMonths(-6);
        var ventesMensuelles = await _context.Ventes
            .Where(v => v.DateVente >= sixMoisAuparavant)
            .Select(v => new { v.DateVente.Year, v.DateVente.Month, v.MontantTotal })
            .ToListAsync();

        var ventesGrouped = ventesMensuelles
            .GroupBy(v => new { v.Year, v.Month })
            .Select(g => new VenteMensuelleDto
            {
                Mois = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy", new System.Globalization.CultureInfo("fr-FR")),
                Montant = g.Sum(v => v.MontantTotal),
                NombreVentes = g.Count()
            })
            .OrderBy(x => x.Mois)
            .ToList();

        dashboard.VentesMensuelles = ventesGrouped;

        // Médicaments les plus vendus (top 5)
        var medicamentsPopulaires = await _context.VenteDetails
            .Include(vd => vd.Medicament)
            .GroupBy(vd => new { vd.MedicamentId, vd.Medicament.Nom })
            .Select(g => new
            {
                MedicamentId = g.Key.MedicamentId,
                Nom = g.Key.Nom ?? "Inconnu",
                QuantiteVendue = g.Sum(vd => (int?)vd.Quantite) ?? 0,
                ChiffreAffaires = g.Sum(vd => (decimal?)vd.SousTotal) ?? 0
            })
            .OrderByDescending(x => x.QuantiteVendue)
            .Take(5)
            .ToListAsync();

        dashboard.MedicamentsPopulaires = medicamentsPopulaires.Select(m => new MedicamentPopulaireDto
        {
            MedicamentId = m.MedicamentId,
            Nom = m.Nom,
            QuantiteVendue = m.QuantiteVendue,
            ChiffreAffaires = m.ChiffreAffaires
        }).ToList();

        // Données pour le caissier : ventes d'aujourd'hui
        var aujourdhui = DateTime.Now.Date;
        var demain = aujourdhui.AddDays(1);
        
        try
        {
            // Essayer avec le filtre Statut (si la colonne existe)
            dashboard.TotalVentesAujourdhui = await _context.Ventes
                .CountAsync(v => v.DateVente >= aujourdhui && v.DateVente < demain && (v.Statut == null || v.Statut == "Normal" || v.Statut == ""));
            
            dashboard.ChiffreAffairesAujourdhui = await _context.Ventes
                .Where(v => v.DateVente >= aujourdhui && v.DateVente < demain && (v.Statut == null || v.Statut == "Normal" || v.Statut == ""))
                .SumAsync(v => (decimal?)v.MontantTotal) ?? 0;
        }
        catch
        {
            // Si la colonne Statut n'existe pas, utiliser sans filtre
            dashboard.TotalVentesAujourdhui = await _context.Ventes
                .CountAsync(v => v.DateVente >= aujourdhui && v.DateVente < demain);
            
            dashboard.ChiffreAffairesAujourdhui = await _context.Ventes
                .Where(v => v.DateVente >= aujourdhui && v.DateVente < demain)
                .SumAsync(v => (decimal?)v.MontantTotal) ?? 0;
        }

        // Médicaments les plus vendus (pour le caissier)
        dashboard.MedicamentsPlusVendus = dashboard.MedicamentsPopulaires;

        // Alertes stock faible (détails)
        dashboard.AlertesStockFaible = medicaments
            .Where(m =>
            {
                var stockDisponible = m.Stocks.Where(s => s.DateExpiration >= DateTime.Now).Sum(s => s.Quantite);
                return stockDisponible <= m.StockMinimum;
            })
            .Select(m => new MedicamentStockFaibleDto
            {
                Id = m.Id,
                Nom = m.Nom,
                StockDisponible = m.Stocks.Where(s => s.DateExpiration >= DateTime.Now).Sum(s => s.Quantite),
                StockMinimum = m.StockMinimum
            })
            .ToList();

        // Ventes récentes (10 dernières)
        List<VenteRecenteDto> ventesRecentes;
        try
        {
            // Essayer avec le filtre Statut (si la colonne existe)
            ventesRecentes = await _context.Ventes
                .Include(v => v.Client)
                .Where(v => v.Statut == null || v.Statut == "Normal" || v.Statut == "")
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
        }
        catch
        {
            // Si la colonne Statut n'existe pas, utiliser sans filtre
            ventesRecentes = await _context.Ventes
                .Include(v => v.Client)
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
        }

        dashboard.VentesRecentes = ventesRecentes;

        return dashboard;
    }
}

