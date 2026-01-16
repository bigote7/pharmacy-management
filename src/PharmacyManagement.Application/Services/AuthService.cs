using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PharmacyManagement.Application.DTOs;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;
using PharmacyManagement.Infrastructure.Repositories;

namespace PharmacyManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username || u.Email == loginDto.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Nom d'utilisateur ou mot de passe incorrect");
        }

        if (!user.EstActif)
        {
            throw new UnauthorizedAccessException("Compte désactivé");
        }

        if (user.Role == null)
        {
            throw new UnauthorizedAccessException("Rôle utilisateur non trouvé");
        }

        // Mettre à jour la dernière connexion
        user.DerniereConnexion = DateTime.Now;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.Now.AddHours(24),
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                RoleName = user.Role.Nom,
                EstActif = user.EstActif
            }
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Vérifier si l'utilisateur existe déjà
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email);

        if (existingUser != null)
        {
            throw new Exception("Un utilisateur avec ce nom d'utilisateur ou cet email existe déjà");
        }

        // Vérifier si le rôle existe
        var role = await _unitOfWork.Roles.GetByIdAsync(registerDto.RoleId);
        if (role == null)
        {
            throw new Exception("Rôle non trouvé");
        }

        // Créer le nouvel utilisateur
        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            FullName = registerDto.FullName,
            RoleId = registerDto.RoleId,
            EstActif = true
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Recharger avec le rôle
        var newUser = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        if (newUser == null)
            throw new Exception("Erreur lors de la création de l'utilisateur");

        var token = GenerateJwtToken(newUser);
        var refreshToken = GenerateRefreshToken();

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.Now.AddHours(24),
            User = new UserDto
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Email = newUser.Email,
                FullName = newUser.FullName,
                RoleName = newUser.Role.Nom,
                EstActif = newUser.EstActif
            }
        };
    }

    public Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        // Implémentation simplifiée - à améliorer avec stockage des refresh tokens
        throw new NotImplementedException("Refresh token à implémenter");
    }

    public Task<bool> LogoutAsync(string token)
    {
        // Implémentation simplifiée - à améliorer avec blacklist de tokens
        return Task.FromResult(true);
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _unitOfWork.Roles.GetAllAsync();
        return roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Nom = r.Nom,
            Description = r.Description
        });
    }

    private string GenerateJwtToken(User user)
    {
        if (user.Role == null)
        {
            throw new Exception("Le rôle de l'utilisateur est null");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "VotreCleSecreteTresLongueEtSecuriseePourJWT123456789"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role.Nom)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "PharmacyManagement",
            audience: _configuration["Jwt:Audience"] ?? "PharmacyManagement",
            claims: claims,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

