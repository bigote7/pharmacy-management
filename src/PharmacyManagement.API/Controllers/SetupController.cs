using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SetupController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SetupController> _logger;

    public SetupController(ApplicationDbContext context, ILogger<SetupController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("create-all-users")]
    public async Task<IActionResult> CreateAllUsers()
    {
        try
        {
            var createdUsers = new List<object>();

            // Créer/vérifier les rôles
            var roles = new[]
            {
                new { Nom = "Administrateur", Description = "Accès complet à tous les modules" },
                new { Nom = "Pharmacien", Description = "Gestion des médicaments, prescriptions et stocks" },
                new { Nom = "Caissier", Description = "Gestion des ventes uniquement" }
            };

            var roleEntities = new List<Role>();
            foreach (var roleData in roles)
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Nom == roleData.Nom);
                if (role == null)
                {
                    role = new Role
                    {
                        Nom = roleData.Nom,
                        Description = roleData.Description
                    };
                    _context.Roles.Add(role);
                    await _context.SaveChangesAsync();
                }
                roleEntities.Add(role);
            }

            // Créer les comptes utilisateurs
            var usersToCreate = new[]
            {
                new { Username = "admin", Email = "admin@pharmacy.ma", Password = "admin123", FullName = "Administrateur", RoleName = "Administrateur" },
                new { Username = "pharmacien", Email = "pharmacien@pharmacy.ma", Password = "pharmacien123", FullName = "Pharmacien Test", RoleName = "Pharmacien" },
                new { Username = "caissier", Email = "caissier@pharmacy.ma", Password = "caissier123", FullName = "Caissier Test", RoleName = "Caissier" }
            };

            foreach (var userData in usersToCreate)
            {
                var role = roleEntities.FirstOrDefault(r => r.Nom == userData.RoleName);
                if (role == null) continue;

                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == userData.Username);
                
                if (existingUser != null)
                {
                    // Réinitialiser le mot de passe
                    existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userData.Password);
                    existingUser.EstActif = true;
                    existingUser.RoleId = role.Id;
                    existingUser.Email = userData.Email;
                    existingUser.FullName = userData.FullName;
                    _context.Users.Update(existingUser);
                    createdUsers.Add(new { username = userData.Username, action = "réinitialisé", role = userData.RoleName });
                }
                else
                {
                    // Créer le compte
                    var newUser = new User
                    {
                        Username = userData.Username,
                        Email = userData.Email,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(userData.Password),
                        FullName = userData.FullName,
                        RoleId = role.Id,
                        EstActif = true,
                        DateCreation = DateTime.Now
                    };
                    _context.Users.Add(newUser);
                    createdUsers.Add(new { username = userData.Username, action = "créé", role = userData.RoleName });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Tous les comptes ont été créés/réinitialisés avec succès",
                users = new[]
                {
                    new { username = "admin", password = "admin123", role = "Administrateur" },
                    new { username = "pharmacien", password = "pharmacien123", role = "Pharmacien" },
                    new { username = "caissier", password = "caissier123", role = "Caissier" }
                },
                details = createdUsers
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création des comptes");
            return BadRequest(new { error = ex.Message, details = ex.ToString() });
        }
    }

    [HttpPost("create-admin")]
    public async Task<IActionResult> CreateAdmin()
    {
        try
        {
            // Vérifier/créer les rôles
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Nom == "Administrateur");
            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Nom = "Administrateur",
                    Description = "Accès complet à tous les modules"
                };
                _context.Roles.Add(adminRole);
                await _context.SaveChangesAsync();
            }

            // Vérifier si l'admin existe
            var existingAdmin = await _context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            
            if (existingAdmin != null)
            {
                // Réinitialiser le mot de passe
                existingAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
                existingAdmin.EstActif = true;
                existingAdmin.RoleId = adminRole.Id;
                _context.Users.Update(existingAdmin);
            }
            else
            {
                // Créer le compte admin
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@pharmacy.ma",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FullName = "Administrateur",
                    RoleId = adminRole.Id,
                    EstActif = true,
                    DateCreation = DateTime.Now
                };
                _context.Users.Add(adminUser);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Compte admin créé/réinitialisé avec succès",
                username = "admin",
                password = "admin123"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du compte admin");
            return BadRequest(new { error = ex.Message, details = ex.ToString() });
        }
    }

    [HttpGet("verify-admin")]
    public async Task<IActionResult> VerifyAdmin()
    {
        try
        {
            var admin = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == "admin");

            if (admin == null)
            {
                return Ok(new { exists = false, message = "Le compte admin n'existe pas" });
            }

            // Tester le hash du mot de passe
            var testPassword = BCrypt.Net.BCrypt.Verify("admin123", admin.PasswordHash);

            return Ok(new
            {
                exists = true,
                username = admin.Username,
                email = admin.Email,
                role = admin.Role?.Nom,
                estActif = admin.EstActif,
                passwordValid = testPassword,
                roleId = admin.RoleId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

