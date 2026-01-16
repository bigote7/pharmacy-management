using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Application.Services;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "PharmacienOrAdmin")] // Le caissier n'a pas accès aux commandes
public class CommandesController : ControllerBase
{
    private readonly ICommandeService _commandeService;

    public CommandesController(ICommandeService commandeService)
    {
        _commandeService = commandeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommandeDto>>> GetCommandes()
    {
        var commandes = await _commandeService.GetAllCommandesAsync();
        return Ok(commandes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommandeDto>> GetCommande(int id)
    {
        var commande = await _commandeService.GetCommandeByIdAsync(id);
        if (commande == null)
            return NotFound();

        return Ok(commande);
    }

    [HttpPost]
    public async Task<ActionResult<CommandeDto>> CreateCommande(CreerCommandeDto dto)
    {
        try
        {
            var commande = await _commandeService.CreateCommandeAsync(dto);
            return CreatedAtAction(nameof(GetCommande), new { id = commande.Id }, commande);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/recevoir")]
    public async Task<ActionResult<CommandeDto>> RecevoirCommande(int id, RecevoirCommandeDto dto)
    {
        try
        {
            var commande = await _commandeService.RecevoirCommandeAsync(id, dto);
            return Ok(commande);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/annuler")]
    public async Task<IActionResult> AnnulerCommande(int id)
    {
        var result = await _commandeService.AnnulerCommandeAsync(id);
        if (!result)
            return BadRequest("Impossible d'annuler cette commande");

        return NoContent();
    }
}

