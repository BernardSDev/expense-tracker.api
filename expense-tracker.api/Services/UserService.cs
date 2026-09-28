using expense_tracker.api.Data;
using expense_tracker.api.DTOs;
using expense_tracker.api.Security;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.api.Services;

public class UserService(AppDbContext context, ICurrentUserService currentUserService) : IUserService
{
    public async Task<ProfileResponseDto> GetProfileAsync()
    {
        var userId = currentUserService.UserId;
        
        var user = await context.Users.FirstOrDefaultAsync(user => user.Id == userId);

        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        return new ProfileResponseDto
        {
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
        };
    }

    public async Task<ProfileResponseDto> UpdateProfileAsync(UpdateProfileDto dto)
    {
        var userId = currentUserService.UserId;

        var user = await context.Users.FirstOrDefaultAsync(user => user.Id == userId);

        if (user is null)
        {
            throw new InvalidOperationException("user not found.");
        }

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;

        await context.SaveChangesAsync();

        return new ProfileResponseDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
        };
    }
}