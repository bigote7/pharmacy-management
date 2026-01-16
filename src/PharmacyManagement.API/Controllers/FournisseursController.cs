using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Application.Services;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "PharmacienOrAdmin")] // Le caissier n'a pas accès aux fournisseurs
public class FournisseursController : ControllerBase
{
    private readonly IFournisseurService _fournisseurService;

    public FournisseursController(IFournisseurService fournisseurService)
    {
        _fournisseurService = fournisseurService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FournisseurDto>>> GetFournisseurs()
    {
        var fournisseurs = await _fournisseurService.GetAllFournisseursAsync();
        return Ok(fournisseurs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FournisseurDto>> GetFournisseur(int id)
    {
        var fournisseur = await _fournisseurService.GetFournisseurByIdAsync(id);
        if (fournisseur == null)
            return NotFound();

        return Ok(fournisseur);
    }

    [HttpPost]
    public async Task<ActionResult<FournisseurDto>> CreateFournisseur(CreerFournisseurDto dto)
    {
        try
        {
            var fournisseur = await _fournisseurService.CreateFournisseurAsync(dto);
            return CreatedAtAction(nameof(GetFournisseur), new { id = fournisseur.Id }, fournisseur);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<FournisseurDto>> UpdateFournisseur(int id, ModifierFournisseurDto dto)
    {
        try
        {
            var fournisseur = await _fournisseurService.UpdateFournisseurAsync(id, dto);
            return Ok(fournisseur);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFournisseur(int id)
    {
        var result = await _fournisseurService.DeleteFournisseurAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

