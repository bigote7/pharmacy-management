using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Tous les endpoints nécessitent une authentification
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        var dashboard = await _dashboardService.GetDashboardDataAsync();
        return Ok(dashboard);
    }

    [HttpGet("caissier")]
    [Authorize(Policy = "CaissierOrAbove")]
    public async Task<ActionResult<object>> GetDashboardCaissier()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var dashboard = await _dashboardService.GetDashboardDataAsync();
            
            // Pour le caissier, retourner seulement les données de ventes
            return Ok(new
            {
                totalVentesAujourdhui = dashboard.TotalVentesAujourdhui,
                chiffreAffairesAujourdhui = dashboard.ChiffreAffairesAujourdhui,
                medicamentsPlusVendus = dashboard.MedicamentsPlusVendus ?? new List<MedicamentPopulaireDto>(),
                alertesStockFaible = dashboard.AlertesStockFaible ?? new List<MedicamentStockFaibleDto>(),
                ventesRecentes = dashboard.VentesRecentes ?? new List<VenteRecenteDto>()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, details = ex.ToString() });
        }
    }
}

