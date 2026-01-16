using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Application.Services;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Tous les endpoints nécessitent une authentification
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionService _prescriptionService;

    public PrescriptionsController(IPrescriptionService prescriptionService)
    {
        _prescriptionService = prescriptionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetPrescriptions()
    {
        var prescriptions = await _prescriptionService.GetAllPrescriptionsAsync();
        return Ok(prescriptions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PrescriptionDto>> GetPrescription(int id)
    {
        var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id);
        if (prescription == null)
            return NotFound();

        return Ok(prescription);
    }

    [HttpPost]
    public async Task<ActionResult<PrescriptionDto>> CreatePrescription(CreerPrescriptionDto dto)
    {
        try
        {
            var prescription = await _prescriptionService.CreatePrescriptionAsync(dto);
            return CreatedAtAction(nameof(GetPrescription), new { id = prescription.Id }, prescription);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/utiliser")]
    public async Task<IActionResult> UtiliserPrescription(int id)
    {
        var result = await _prescriptionService.UtiliserPrescriptionAsync(id);
        if (!result)
            return BadRequest("Impossible d'utiliser cette prescription");

        return NoContent();
    }
}

