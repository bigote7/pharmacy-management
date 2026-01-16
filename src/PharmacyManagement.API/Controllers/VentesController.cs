using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Tous les endpoints nécessitent une authentification
public class VentesController : ControllerBase
{
    private readonly IVenteService _venteService;

    public VentesController(IVenteService venteService)
    {
        _venteService = venteService;
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userIdClaim != null && int.TryParse(userIdClaim, out int userId) ? userId : null;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VenteDto>>> GetVentes()
    {
        var userId = GetCurrentUserId();
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        
        // Si c'est un caissier, ne montrer que ses ventes
        if (userRole == "Caissier" && userId.HasValue)
        {
            var ventes = await _venteService.GetVentesByUserIdAsync(userId.Value);
            return Ok(ventes);
        }
        
        var allVentes = await _venteService.GetAllVentesAsync();
        return Ok(allVentes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VenteDto>> GetVente(int id)
    {
        var vente = await _venteService.GetVenteByIdAsync(id);
        if (vente == null)
            return NotFound();

        return Ok(vente);
    }

    [HttpPost]
    public async Task<ActionResult<VenteDto>> CreateVente(CreerVenteDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var vente = await _venteService.CreateVenteAsync(dto, userId);
            return CreatedAtAction(nameof(GetVente), new { id = vente.Id }, vente);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/annuler")]
    public async Task<IActionResult> AnnulerVente(int id, [FromBody] AnnulerVenteDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _venteService.AnnulerVenteAsync(id, dto.Raison, userId);
            if (!result)
                return BadRequest("Impossible d'annuler cette vente");
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/retourner")]
    public async Task<IActionResult> RetournerVente(int id, [FromBody] RetournerVenteDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _venteService.RetournerVenteAsync(id, dto, userId);
            if (!result)
                return BadRequest("Impossible de retourner cette vente");
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("calculer-total")]
    public async Task<ActionResult<decimal>> CalculerTotal(List<CreerVenteDetailDto> details)
    {
        try
        {
            var total = await _venteService.CalculerTotalAsync(details);
            return Ok(new { total });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("valider-stock")]
    public async Task<ActionResult<bool>> ValiderStock([FromBody] ValiderStockDto dto)
    {
        var isValid = await _venteService.ValiderStockDisponibleAsync(dto.MedicamentId, dto.Quantite);
        return Ok(new { isValid });
    }
}

public class ValiderStockDto
{
    public int MedicamentId { get; set; }
    public int Quantite { get; set; }
}

