using PharmacyManagement.Application.DTOs;

namespace PharmacyManagement.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(string token);
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
}

