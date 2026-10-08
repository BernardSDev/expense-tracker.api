using expense_tracker.api.Constants;
using expense_tracker.api.Data;
using expense_tracker.api.DTOs;
using expense_tracker.api.DTOs.Categories;
using expense_tracker.api.DTOs.Expenses;
using expense_tracker.api.Exceptions;
using expense_tracker.api.Models;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.api.Services;

public class CategoryService(AppDbContext context) : ICategoryService
{
    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto, Guid userId)
    {
        var category = new Category
        {
            Name = dto.Name,
            UserId = userId
        };
        
        context.Categories.Add(category);

        await context.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
        };
    }
    
    public async Task<List<CategoryResponseDto>> GetAllAsync(Guid userId)
    {
        return await context.Categories
            .Where(category => category.UserId == userId)
            .Select(category => new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name
            })
            .ToListAsync();
    }
    
    public async Task<CategoryResponseDto?> UpdateAsync(int id, UpdateCategoryDto dto, Guid userId)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(category =>
                category.Id == id &&
                category.UserId == userId);

        if (category is null)
            return null;

        category.Name = dto.Name;

        await context.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }
    
    public async Task<bool> DeleteAsync(int id, Guid userId)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.UserId == userId);

        if (category == null)
        {
            return false;
        }

        var isInUse = await context.Expenses
            .AnyAsync(e => e.CategoryId == id);

        if (isInUse)
        {
            throw new ConflictException(
                ErrorMessages.CategoryInUse
            );
        }

        context.Categories.Remove(category);

        await context.SaveChangesAsync();

        return true;
    }
}