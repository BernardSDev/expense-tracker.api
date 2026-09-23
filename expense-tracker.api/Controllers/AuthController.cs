using expense_tracker.api.DTOs;
using expense_tracker.api.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace expense_tracker.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    
    public async Task<ActionResult<RegistrationResponseDto>> Register(RegistrationDto dto)
    {
        var result = await authService.RegisterAsync(dto);
        
        return StatusCode(StatusCodes.Status201Created, result);
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);

        return Ok(result);
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto)
    {
        var response = await authService.RefreshAsync(dto.RefreshToken);

        return Ok(response);
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequestDto dto)
    {
        await authService.LogoutAsync(dto.RefreshToken);

        return NoContent();
    }
}