using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Application.Services;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Tous les endpoints nécessitent une authentification
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients([FromQuery] string? search = null)
    {
        IEnumerable<ClientDto> clients;
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            clients = await _clientService.SearchClientsAsync(search);
        }
        else
        {
            clients = await _clientService.GetAllClientsAsync();
        }
        
        return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetClient(int id)
    {
        var client = await _clientService.GetClientByIdAsync(id);
        if (client == null)
            return NotFound();

        return Ok(client);
    }

    [HttpGet("search/{searchTerm}")]
    public async Task<ActionResult<IEnumerable<ClientDto>>> SearchClients(string searchTerm)
    {
        var clients = await _clientService.SearchClientsAsync(searchTerm);
        return Ok(clients);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> CreateClient(CreerClientDto dto)
    {
        try
        {
            var client = await _clientService.CreateClientAsync(dto);
            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClientDto>> UpdateClient(int id, ModifierClientDto dto)
    {
        try
        {
            var client = await _clientService.UpdateClientAsync(id, dto);
            return Ok(client);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "PharmacienOrAdmin")] // Seul le pharmacien/admin peut supprimer
    public async Task<IActionResult> DeleteClient(int id)
    {
        var result = await _clientService.DeleteClientAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

