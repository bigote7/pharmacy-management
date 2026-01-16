using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Application.Services;
using PharmacyManagement.API.Validators;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Tous les endpoints nécessitent une authentification
public class MedicamentsController : ControllerBase
{
    private readonly IMedicamentService _medicamentService;

    public MedicamentsController(IMedicamentService medicamentService)
    {
        _medicamentService = medicamentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicamentDto>>> GetMedicaments([FromQuery] string? search = null)
    {
        var medicaments = await _medicamentService.GetAllMedicamentsAsync();
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            medicaments = medicaments.Where(m => 
                m.Nom.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                m.CodeBarre.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                m.CategorieNom.Contains(search, StringComparison.OrdinalIgnoreCase)
            );
        }
        
        return Ok(medicaments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MedicamentDto>> GetMedicament(int id)
    {
        var medicament = await _medicamentService.GetMedicamentByIdAsync(id);
        if (medicament == null)
            return NotFound();

        return Ok(medicament);
    }

    [HttpGet("codebarre/{codeBarre}")]
    public async Task<ActionResult<MedicamentDto>> GetMedicamentByCodeBarre(string codeBarre)
    {
        var medicament = await _medicamentService.GetMedicamentByCodeBarreAsync(codeBarre);
        if (medicament == null)
            return NotFound();

        return Ok(medicament);
    }

    [HttpGet("stock-faible")]
    public async Task<ActionResult<IEnumerable<MedicamentDto>>> GetMedicamentsStockFaible()
    {
        var medicaments = await _medicamentService.GetMedicamentsStockFaibleAsync();
        return Ok(medicaments);
    }

    [HttpPost]
    [Authorize(Policy = "PharmacienOrAdmin")]
    public async Task<ActionResult<MedicamentDto>> CreateMedicament([FromBody] CreerMedicamentDto dto)
    {
        if (dto == null)
        {
            return BadRequest(new { error = "Les données du médicament sont requises" });
        }

        // Validation
        var validationErrors = CreerMedicamentValidator.Validate(dto);
        if (validationErrors.Any())
        {
            return BadRequest(new { errors = validationErrors });
        }

        try
        {
            var medicament = await _medicamentService.CreateMedicamentAsync(dto);
            return CreatedAtAction(nameof(GetMedicament), new { id = medicament.Id }, medicament);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "PharmacienOrAdmin")]
    public async Task<ActionResult<MedicamentDto>> UpdateMedicament(int id, ModifierMedicamentDto dto)
    {
        try
        {
            var medicament = await _medicamentService.UpdateMedicamentAsync(id, dto);
            return Ok(medicament);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "PharmacienOrAdmin")]
    public async Task<IActionResult> DeleteMedicament(int id)
    {
        var result = await _medicamentService.DeleteMedicamentAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

