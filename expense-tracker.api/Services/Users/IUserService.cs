using expense_tracker.api.DTOs;
using expense_tracker.api.DTOs.Users;

namespace expense_tracker.api.Services;

public interface IUserService
{
    Task<ProfileResponseDto> GetProfileAsync();
    Task<ProfileResponseDto> UpdateProfileAsync(UpdateProfileDto dto);
}