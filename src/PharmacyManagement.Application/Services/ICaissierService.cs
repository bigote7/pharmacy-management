using PharmacyManagement.Application.DTOs;

namespace PharmacyManagement.Application.Services;

public interface ICaissierService
{
    // Dashboard
    Task<DashboardCaissierViewModel> GetDashboardAsync(int userId);
    
    // Vente
    Task<IEnumerable<RechercheMedicamentDto>> GetAllMedicamentsAsync();
    Task<IEnumerable<RechercheMedicamentDto>> RechercherMedicamentsAsync(string terme);
    Task<RechercheMedicamentDto?> RechercherMedicamentParCodeBarreAsync(string codeBarre);
    Task<VenteCompleteViewModel> CreerVenteAsync(VenteCaissierViewModel vente, int userId);
    Task<bool> ValiderStockAsync(int medicamentId, int quantite);
    Task<decimal> CalculerTotalAvecTVAAsync(List<LigneVenteViewModel> lignes);
    
    // Historique
    Task<IEnumerable<HistoriqueVenteViewModel>> GetHistoriqueVentesAsync(int userId, DateTime? dateDebut = null, DateTime? dateFin = null);
    Task<VenteCompleteViewModel> GetVenteDetailsAsync(int venteId, int userId);
    Task<bool> AnnulerVenteAsync(int venteId, string raison, int userId);
    
    // Clients
    Task<IEnumerable<ClientDto>> RechercherClientsAsync(string terme);
    Task<ClientDto?> GetClientAsync(int clientId);
    
    // Alertes (lecture seule)
    Task<IEnumerable<AlerteStockFaibleDto>> GetAlertesStockFaibleAsync();
    Task<IEnumerable<StockDto>> GetStocksExpirantBientotAsync();
    
    // Impression
    Task<byte[]> GenererFacturePDFAsync(int venteId);
    Task<string> GenererTicket80mmAsync(int venteId);
}

