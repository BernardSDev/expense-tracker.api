using expense_tracker.api.DTOs;

namespace expense_tracker.api.Services;

public interface ICategoryService
{
    Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto, Guid userId);

    Task<List<CategoryResponseDto>> GetAllAsync(Guid userId);

    Task<CategoryResponseDto?> UpdateAsync(int id, UpdateCategoryDto dto, Guid userId);

    Task<bool> DeleteAsync(int id, Guid userId);
}