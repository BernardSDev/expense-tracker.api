using expense_tracker.api.Data;
using expense_tracker.api.DTOs;
using expense_tracker.api.Models;
using expense_tracker.api.Security;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.api.Services.Auth;

public class AuthService(
    AppDbContext context, 
    IPasswordHasher passwordHasher, 
    IJwtTokenService jwtTokenService
    ) : IAuthService
{
    public async Task<RegistrationResponseDto> RegisterAsync(RegistrationDto dto)
    {
        var usernameExists = await context.Users.AnyAsync(user => user.Username == dto.Username);
        var emailExists = await context.Users.AnyAsync(user => user.Email == dto.Email);

        if (usernameExists)
        {
            throw new InvalidOperationException("Username is  already taken");
        }
        
        if (emailExists)
        {
            throw new InvalidOperationException("Email is already in use.");
        }

        var passwordHash = passwordHasher.HashPassword(dto.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = passwordHash,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        context.Users.Add(user);
        
        await context.SaveChangesAsync();

        return new RegistrationResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await context.Users.FirstOrDefaultAsync(user => user.Username == dto.Username);

        if (user  is null)
        {
            throw new InvalidOperationException("Invalid username or password.");
        }

        if (user.PasswordHash is null)
        {
            throw new InvalidOperationException("Invalid username or password.");
        }

        var passwordIsValid = passwordHasher.VerifyPassword(dto.Password, user.PasswordHash);

        if (!passwordIsValid)
        {
            throw new InvalidOperationException("Invalid username or password.");
        }

        var accessToken = jwtTokenService.CreateAccessToken(user);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
        };
    }
}