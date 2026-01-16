using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("reset-admin")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetAdminPassword()
    {
        try
        {
            var admin = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == "admin");

            if (admin == null)
            {
                // Créer l'admin s'il n'existe pas
                var adminRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Nom == "Administrateur");

                if (adminRole == null)
                {
                    return BadRequest(new { error = "Le rôle Administrateur n'existe pas" });
                }

                admin = new Domain.Entities.User
                {
                    Username = "admin",
                    Email = "admin@pharmacy.ma",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FullName = "Administrateur",
                    RoleId = adminRole.Id,
                    EstActif = true
                };

                _context.Users.Add(admin);
            }
            else
            {
                // Réinitialiser le mot de passe
                admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
                admin.EstActif = true;
                _context.Users.Update(admin);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Compte admin réinitialisé avec succès. Username: admin, Password: admin123" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("check-admin")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckAdmin()
    {
        var admin = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == "admin");

        if (admin == null)
        {
            return Ok(new { exists = false, message = "Le compte admin n'existe pas" });
        }

        return Ok(new
        {
            exists = true,
            username = admin.Username,
            email = admin.Email,
            role = admin.Role?.Nom,
            estActif = admin.EstActif
        });
    }
}

