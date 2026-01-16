using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "CaissierOrAbove")]
public class CaissierController : ControllerBase
{
    private readonly ICaissierService _caissierService;
    private readonly ILogger<CaissierController> _logger;

    public CaissierController(ICaissierService caissierService, ILogger<CaissierController> logger)
    {
        _caissierService = caissierService;
        _logger = logger;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            throw new UnauthorizedAccessException("Utilisateur non authentifié");
        return userId;
    }

    // Dashboard
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardCaissierViewModel>> GetDashboard()
    {
        try
        {
            var userId = GetCurrentUserId();
            var dashboard = await _caissierService.GetDashboardAsync(userId);
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du chargement du dashboard caissier");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Tous les médicaments
    [HttpGet("medicaments")]
    [AllowAnonymous] // Temporaire pour tester le routage
    public async Task<ActionResult<IEnumerable<RechercheMedicamentDto>>> GetAllMedicaments()
    {
        try
        {
            _logger.LogInformation("Endpoint /api/caissier/medicaments appelé");
            var medicaments = await _caissierService.GetAllMedicamentsAsync();
            _logger.LogInformation($"Retour de {medicaments.Count()} médicaments");
            return Ok(medicaments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du chargement des médicaments");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Recherche médicament
    [HttpGet("medicaments/rechercher")]
    public async Task<ActionResult<IEnumerable<RechercheMedicamentDto>>> RechercherMedicament([FromQuery] string terme)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(terme))
                return BadRequest(new { error = "Le terme de recherche est requis" });

            var medicaments = await _caissierService.RechercherMedicamentsAsync(terme);
            return Ok(medicaments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la recherche de médicament");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Recherche par code-barres
    [HttpGet("medicaments/codebarre/{codeBarre}")]
    public async Task<ActionResult<RechercheMedicamentDto>> RechercherParCodeBarre(string codeBarre)
    {
        try
        {
            var medicament = await _caissierService.RechercherMedicamentParCodeBarreAsync(codeBarre);
            if (medicament == null)
                return NotFound(new { error = "Médicament non trouvé" });

            return Ok(medicament);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la recherche par code-barres");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Validation stock
    [HttpPost("valider-stock")]
    public async Task<ActionResult<bool>> ValiderStock([FromBody] ValiderStockRequest request)
    {
        try
        {
            var isValid = await _caissierService.ValiderStockAsync(request.MedicamentId, request.Quantite);
            return Ok(new { isValid, message = isValid ? "Stock disponible" : "Stock insuffisant" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation du stock");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Calculer total
    [HttpPost("calculer-total")]
    public async Task<ActionResult<PaiementViewModel>> CalculerTotal([FromBody] List<LigneVenteViewModel> lignes)
    {
        try
        {
            var totalAvecTVA = await _caissierService.CalculerTotalAvecTVAAsync(lignes);
            var montantHT = lignes.Sum(l => l.SousTotal);
            var montantTVA = totalAvecTVA - montantHT;

            return Ok(new PaiementViewModel
            {
                MontantHT = montantHT,
                MontantTVA = montantTVA,
                MontantTotal = totalAvecTVA
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du calcul du total");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Créer vente
    [HttpPost("ventes")]
    public async Task<ActionResult<VenteCompleteViewModel>> CreerVente([FromBody] VenteCaissierViewModel vente)
    {
        try
        {
            // Validations - Vérifier que le modèle est bien reçu
            if (vente == null)
                return BadRequest(new { error = "Les données de vente sont requises" });
            
            // Le binding JSON devrait mapper "lignes" (camelCase) à "Lignes" (PascalCase)
            // Mais vérifions les deux cas pour être sûr
            var lignes = vente.Lignes ?? new List<LigneVenteViewModel>();
            
            if (lignes == null || !lignes.Any())
            {
                _logger.LogWarning("Tentative de création de vente avec panier vide. Données reçues: {VenteData}", 
                    System.Text.Json.JsonSerializer.Serialize(vente));
                return BadRequest(new { error = "Le panier ne peut pas être vide" });
            }

            // Valider chaque ligne
            foreach (var ligne in lignes)
            {
                var stockValide = await _caissierService.ValiderStockAsync(ligne.MedicamentId, ligne.Quantite);
                if (!stockValide)
                    return BadRequest(new { error = $"Stock insuffisant pour {ligne.MedicamentNom}" });
            }

            // S'assurer que vente.Lignes est défini pour le service
            vente.Lignes = lignes;
            
            var userId = GetCurrentUserId();
            var venteCreee = await _caissierService.CreerVenteAsync(vente, userId);
            
            return CreatedAtAction(nameof(GetVenteDetails), new { id = venteCreee.VenteId }, venteCreee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création de la vente");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Historique des ventes
    [HttpGet("ventes/historique")]
    public async Task<ActionResult<IEnumerable<HistoriqueVenteViewModel>>> GetHistorique(
        [FromQuery] DateTime? dateDebut = null,
        [FromQuery] DateTime? dateFin = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            var historique = await _caissierService.GetHistoriqueVentesAsync(userId, dateDebut, dateFin);
            return Ok(historique);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du chargement de l'historique");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Détails d'une vente
    [HttpGet("ventes/{id}")]
    public async Task<ActionResult<VenteCompleteViewModel>> GetVenteDetails(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var vente = await _caissierService.GetVenteDetailsAsync(id, userId);
            return Ok(vente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du chargement des détails de la vente");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Annuler vente
    [HttpPost("ventes/{id}/annuler")]
    public async Task<IActionResult> AnnulerVente(int id, [FromBody] AnnulerVenteRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _caissierService.AnnulerVenteAsync(id, request.Raison, userId);
            
            if (!result)
                return BadRequest(new { error = "Impossible d'annuler cette vente" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'annulation de la vente");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Recherche clients
    [HttpGet("clients/rechercher")]
    public async Task<ActionResult<IEnumerable<ClientDto>>> RechercherClients([FromQuery] string terme)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(terme))
                return BadRequest(new { error = "Le terme de recherche est requis" });

            var clients = await _caissierService.RechercherClientsAsync(terme);
            return Ok(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la recherche de clients");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Get client
    [HttpGet("clients/{id}")]
    public async Task<ActionResult<ClientDto>> GetClient(int id)
    {
        try
        {
            var client = await _caissierService.GetClientAsync(id);
            if (client == null)
                return NotFound(new { error = "Client non trouvé" });

            return Ok(client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du chargement du client");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Alertes stock faible
    [HttpGet("alertes/stock-faible")]
    public async Task<ActionResult<IEnumerable<AlerteStockFaibleDto>>> GetAlertesStockFaible()
    {
        try
        {
            var alertes = await _caissierService.GetAlertesStockFaibleAsync();
            return Ok(alertes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du chargement des alertes");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Stocks expirant bientôt
    [HttpGet("alertes/expiration")]
    public async Task<ActionResult<IEnumerable<StockDto>>> GetStocksExpirantBientot()
    {
        try
        {
            var stocks = await _caissierService.GetStocksExpirantBientotAsync();
            return Ok(stocks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du chargement des stocks expirant");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Générer facture PDF
    [HttpGet("ventes/{id}/facture-pdf")]
    public async Task<IActionResult> GenererFacturePDF(int id)
    {
        try
        {
            var pdfBytes = await _caissierService.GenererFacturePDFAsync(id);
            return File(pdfBytes, "application/pdf", $"Facture_{id}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la génération du PDF");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Générer ticket 80mm
    [HttpGet("ventes/{id}/ticket")]
    public async Task<ActionResult<string>> GenererTicket(int id)
    {
        try
        {
            var ticket = await _caissierService.GenererTicket80mmAsync(id);
            return Ok(new { ticket });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la génération du ticket");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

// DTOs pour les requêtes
public class ValiderStockRequest
{
    public int MedicamentId { get; set; }
    public int Quantite { get; set; }
}

public class AnnulerVenteRequest
{
    public string Raison { get; set; } = string.Empty;
}

