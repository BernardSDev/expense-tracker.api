using expense_tracker.api.DTOs;
using expense_tracker.api.DTOs.Auth;

namespace expense_tracker.api.Services.Auth;

public interface IAuthService
{
    Task<RegistrationResponseDto> RegisterAsync(RegistrationDto dto);
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
    Task<LoginResponseDto> RefreshAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
}