using expense_tracker.api.Constants;
using expense_tracker.api.DTOs;
using expense_tracker.api.DTOs.Categories;
using expense_tracker.api.DTOs.Expenses;
using expense_tracker.api.Extensions;
using expense_tracker.api.Security;
using expense_tracker.api.Security.CurrentUser;
using expense_tracker.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace expense_tracker.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(
    ICategoryService categoryService,
    ICurrentUserService currentUserService
) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
    {
        var userId = currentUserService.UserId;

        var category = await categoryService.CreateAsync(dto, userId);

        return Ok(category);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = currentUserService.UserId;

        var categories = await categoryService.GetAllAsync(userId);

        return Ok(categories);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateCategoryDto dto)
    {
        var userId = currentUserService.UserId;

        var category = await categoryService.UpdateAsync(
            id,
            dto,
            userId);

        if (category is null)
            return this.NotFoundError(ErrorMessages.CategoryNotFound);

        return Ok(category);
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = currentUserService.UserId;

        var deleted = await categoryService.DeleteAsync(id, userId);

        if (!deleted)
            return this.NotFoundError(ErrorMessages.CategoryNotFound);

        return NoContent();
    }
}