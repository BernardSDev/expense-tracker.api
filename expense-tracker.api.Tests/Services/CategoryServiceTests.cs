using expense_tracker.api.DTOs.Categories;
using expense_tracker.api.DTOs.Expenses;
using expense_tracker.api.Services;
using expense_tracker.api.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class CategoryServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateCategory_WhenDataIsValid()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;

        var user = await TestData.CreateUserAsync(
            context,
            "category-user",
            "category@example.com");

        var dto = new CreateCategoryDto
        {
            Name = "Groceries"
        };

        var categoryService = new CategoryService(context);

        // Act
        var result = await categoryService.CreateAsync(dto, user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Groceries", result.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyUserCategories()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;

        var user = await TestData.CreateUserAsync(
            context,
            "category-user",
            "category@example.com");

        var otherUser = await TestData.CreateUserAsync(
            context,
            "other-category-user",
            "other-category@example.com");
        
        var categoryService = new CategoryService(context);
        
        var category1 = await categoryService.CreateAsync(
            new CreateCategoryDto { Name = "Groceries" },
            user.Id);

        var category2 = await categoryService.CreateAsync(
            new CreateCategoryDto { Name = "Transport" },
            user.Id);

        var otherUserCategory = await categoryService.CreateAsync(
            new CreateCategoryDto { Name = "Business" },
            otherUser.Id);
        
        // Act
        var result = await categoryService.GetAllAsync(user.Id);
        
        // Assert
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Contains(result, c => c.Name == "Groceries");
        Assert.Contains(result, c => c.Name == "Transport");
        Assert.DoesNotContain(result, c => c.Name == "Business");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenUserHasNoCategories()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;

        var user = await TestData.CreateUserAsync(
            context,
            "empty-category-user",
            "empty-category@example.com");
        
        // Act
        var categoryService = new CategoryService(context);
        var result = await categoryService.GetAllAsync(user.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateCategory_WhenCategoryBelongsToUser()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;

        var user = await TestData.CreateUserAsync(
            context,
            "update-category-user",
            "update-category@example.com");

        var categoryService = new CategoryService(context);
        
        var category = await categoryService.CreateAsync(
            new CreateCategoryDto
            {
                Name = "Groceries"
            }, 
            user.Id);
        
        var dto = new UpdateCategoryDto
        {
            Name = "Food"
        };
        
        // Act
        var result = await categoryService.UpdateAsync(category.Id, dto, user.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal("Food", result.Name);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;

        var user = await TestData.CreateUserAsync(
            context,
            "missing-category-user",
            "missing-category@example.com");

        var dto = new UpdateCategoryDto
        {
            Name = "Food"
        };

        var categoryService = new CategoryService(context);

        // Act
        var result = await categoryService.UpdateAsync(
            999999,
            dto,
            user.Id);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenCategoryBelongsToAnotherUser()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;

        var owner = await TestData.CreateUserAsync(
            context,
            "category-owner",
            "category-owner@example.com");

        var otherUser = await TestData.CreateUserAsync(
            context,
            "other-category-user",
            "other-category@example.com");

        var categoryService = new CategoryService(context);

        var category = await categoryService.CreateAsync(
            new CreateCategoryDto
            {
                Name = "Groceries"
            },
            owner.Id);

        var dto = new UpdateCategoryDto
        {
            Name = "Unauthorized Update"
        };

        // Act
        var result = await categoryService.UpdateAsync(
            category.Id,
            dto,
            otherUser.Id);

        // Assert
        Assert.Null(result);

        var unchangedCategory = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == category.Id);

        Assert.NotNull(unchangedCategory);
        Assert.Equal("Groceries", unchangedCategory.Name);
        Assert.Equal(owner.Id, unchangedCategory.UserId);
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenCategoryExists()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;

        var user = await TestData.CreateUserAsync(
            context,
            "delete-category-user",
            "delete-category@example.com");

        var categoryService = new CategoryService(context);

        var category = await categoryService.CreateAsync(
            new CreateCategoryDto
            {
                Name = "Groceries"
            },
            user.Id);

        // Act
        var result = await categoryService.DeleteAsync(
            category.Id,
            user.Id);

        // Assert
        Assert.True(result);

        var deletedCategory = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == category.Id);

        Assert.Null(deletedCategory);
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenCategoryDoesNotExist()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;

        var user = await TestData.CreateUserAsync(
            context,
            "missing-delete-category-user",
            "missing-delete-category@example.com");

        var categoryService = new CategoryService(context);

        // Act
        var result = await categoryService.DeleteAsync(
            999999,
            user.Id);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenCategoryBelongsToAnotherUser()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;

        var owner = await TestData.CreateUserAsync(
            context,
            "delete-category-owner",
            "delete-category-owner@example.com");

        var otherUser = await TestData.CreateUserAsync(
            context,
            "other-delete-category-user",
            "other-delete-category@example.com");

        var categoryService = new CategoryService(context);

        var category = await categoryService.CreateAsync(
            new CreateCategoryDto
            {
                Name = "Groceries"
            },
            owner.Id);

        // Act
        var result = await categoryService.DeleteAsync(
            category.Id,
            otherUser.Id);

        // Assert
        Assert.False(result);

        var existingCategory = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == category.Id);

        Assert.NotNull(existingCategory);
        Assert.Equal(owner.Id, existingCategory.UserId);
    }
}