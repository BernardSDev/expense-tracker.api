using expense_tracker.api.DTOs;

namespace expense_tracker.api.Services.Auth;

public interface IAuthService
{
    Task<RegistrationResponseDto> RegisterAsync(RegistrationDto dto);
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
    Task<LoginResponseDto> RefreshAsync(string refreshToken);
}