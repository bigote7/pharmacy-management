using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Application.Services;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Tous les endpoints nécessitent une authentification
public class StocksController : ControllerBase
{
    private readonly IStockService _stockService;

    public StocksController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockDto>>> GetStocks()
    {
        var stocks = await _stockService.GetAllStocksAsync();
        return Ok(stocks);
    }

    [HttpGet("medicament/{medicamentId}")]
    public async Task<ActionResult<IEnumerable<StockDto>>> GetStocksByMedicament(int medicamentId)
    {
        var stocks = await _stockService.GetStocksByMedicamentIdAsync(medicamentId);
        return Ok(stocks);
    }

    [HttpGet("expires")]
    public async Task<ActionResult<IEnumerable<StockDto>>> GetStocksExpires()
    {
        var stocks = await _stockService.GetStocksExpiresAsync();
        return Ok(stocks);
    }

    [HttpGet("expire-bientot")]
    public async Task<ActionResult<IEnumerable<StockDto>>> GetStocksExpireBientot()
    {
        var stocks = await _stockService.GetStocksExpireBientotAsync();
        return Ok(stocks);
    }

    [HttpPost]
    [Authorize(Policy = "PharmacienOrAdmin")]
    public async Task<ActionResult<StockDto>> CreateStock(CreerStockDto dto)
    {
        try
        {
            var stock = await _stockService.CreateStockAsync(dto);
            return CreatedAtAction(nameof(GetStocks), new { id = stock.Id }, stock);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "PharmacienOrAdmin")]
    public async Task<ActionResult<StockDto>> UpdateStock(int id, ModifierStockDto dto)
    {
        try
        {
            var stock = await _stockService.UpdateStockAsync(id, dto);
            return Ok(stock);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "PharmacienOrAdmin")]
    public async Task<IActionResult> DeleteStock(int id)
    {
        var result = await _stockService.DeleteStockAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

