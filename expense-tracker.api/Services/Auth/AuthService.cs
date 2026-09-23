using expense_tracker.api.Data;
using expense_tracker.api.DTOs;
using expense_tracker.api.Models;
using expense_tracker.api.Security;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.api.Services.Auth;

public class AuthService(
    AppDbContext context, 
    IPasswordHasher passwordHasher, 
    IJwtTokenService jwtTokenService,
    IRefreshTokenGenerator refreshTokenGenerator,
    IRefreshTokenHasher refreshTokenHasher,
    IConfiguration configuration
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
        
        var refreshToken = refreshTokenGenerator.GenerateRefreshToken();
        
        var tokenHash = refreshTokenHasher.Hash(refreshToken);

        var now = DateTimeOffset.UtcNow;

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            CreatedAt = now,
            ExpiresAt = now.AddDays(configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays")),
            RevokedAt = null
        };
        
        context.RefreshTokens.Add(refreshTokenEntity);
        
        await context.SaveChangesAsync();

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<LoginResponseDto> RefreshAsync(string refreshToken)
    {
        var tokenHash = refreshTokenHasher.Hash(refreshToken);
        
        var storedToken = await context.RefreshTokens.FirstOrDefaultAsync(token => token.TokenHash == tokenHash);

        if (storedToken is null)
        {
            throw new InvalidOperationException("Invalid refresh token.");
        }

        if (storedToken.ExpiresAt < DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException("Refresh token has expired.");
        }

        if (storedToken.RevokedAt is not null)
        {
            throw new InvalidOperationException("Refresh token has already been revoked.");
        }
        
        var user = await context.Users.FirstOrDefaultAsync(user => user.Id == storedToken.UserId);

        if (user is null)
        {
            throw new InvalidOperationException("User associated with refresh token was not found.");
        }
        
        storedToken.RevokedAt = DateTimeOffset.UtcNow;
        
        var newRefreshToken = refreshTokenGenerator.GenerateRefreshToken();
        var newTokenHash = refreshTokenHasher.Hash(newRefreshToken);
        
        var now = DateTimeOffset.UtcNow;

        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = newTokenHash,
            CreatedAt = now,
            ExpiresAt = now.AddDays(configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays")),
            RevokedAt = null
        };
        
        context.RefreshTokens.Add(newRefreshTokenEntity);
        
        await context.SaveChangesAsync();
        
        var accessToken = jwtTokenService.CreateAccessToken(user);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        };
    }
    
    public async Task LogoutAsync(string refreshToken)
    {
        var tokenHash = refreshTokenHasher.Hash(refreshToken);
        
        var storedToken = context.RefreshTokens.FirstOrDefault(token => token.TokenHash == tokenHash);

        if (storedToken is null)
        {
            throw new InvalidOperationException("Invalid refresh token.");
        }
        
        storedToken.RevokedAt = DateTimeOffset.UtcNow;
        
        await context.SaveChangesAsync();
    }
}