using expense_tracker.api.DTOs;

namespace expense_tracker.api.Services;

public interface IUserService
{
    Task<ProfileResponseDto> GetProfileAsync();
    Task<ProfileResponseDto> UpdateProfileAsync(UpdateProfileDto dto);
}