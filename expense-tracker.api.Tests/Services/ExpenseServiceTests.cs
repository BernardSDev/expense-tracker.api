using expense_tracker.api.DTOs.Categories;
using expense_tracker.api.DTOs.Expenses;
using expense_tracker.api.Models;
using expense_tracker.api.Services;
using expense_tracker.api.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace expense_tracker.api.Tests.Services;

public class ExpenseServiceTests
{
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenExpenseDoesNotExist()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;
        
        var expenseService = new ExpenseService(context);

        // Act
        var result = await expenseService.DeleteAsync(999999, Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_AndDeleteExpense_WhenExpenseExists()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;
    
        var user = await TestData.CreateUserAsync(
            context,
            "testuser",
            "test@example.com");
    
        var expense = await TestData.CreateExpenseAsync(context, user.Id);
        var expenseService = new ExpenseService(context);
    
        // Act
        var result = await expenseService.DeleteAsync(expense.Id, user.Id);
    
        // Assert
        Assert.True(result);
    
        var deletedExpense = await context.Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == expense.Id);
    
        Assert.Null(deletedExpense);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldCreateExpense_WhenDataIsValid()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;
        
        var user = await TestData.CreateUserAsync( 
            context,
            "create-test-user",
            "create-test@example.com");
        
        var dto = new CreateExpenseDto
        {
            Amount = 250,
            Description = "Lunch",
            Date = DateTime.UtcNow
        };
        
        var expenseService = new ExpenseService(context);
        
        // Act
        var result = await expenseService.CreateAsync(dto, user.Id);
    
        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal(dto.Amount, result.Amount);
        Assert.Equal(dto.Description, result.Description);
        Assert.Equal(dto.Date, result.Date);
        Assert.Equal(user.Id, result.UserId);
    
        var createdExpense = await context.Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == result.Id);

        Assert.NotNull(createdExpense);
        Assert.Equal(dto.Amount, createdExpense.Amount);
        Assert.Equal(dto.Description, createdExpense.Description);
        Assert.Equal(dto.Date, createdExpense.Date);
        Assert.Equal(user.Id, createdExpense.UserId);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenDateIsMissing()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;
        
        var user = await TestData.CreateUserAsync(
            context,
            "missing-date-user",
            "missing-date@example.com");

        var dto = new CreateExpenseDto
        {
            Amount = 100,
            Description = "Missing date",
            Date = null
        };

        var expenseService = new ExpenseService(context);
        
      
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => expenseService.CreateAsync(dto, user.Id));

        Assert.Equal("Date is required.", exception.Message);
     
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnExpense_WhenExpenseBelongsToUser()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;
        
        var user = await TestData.CreateUserAsync(
            context,
            "get-by-id-user",
            "get-by-id@example.com");

        var expense = await TestData.CreateExpenseAsync(
            context,
            user.Id,
            150,
            "Groceries");
        
        var expenseService = new ExpenseService(context);
        
        // Act
        var result = await expenseService.GetByIdAsync(expense.Id, user.Id);
    
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expense.Id, result.Id);
        Assert.Equal(expense.Amount, result.Amount);
        Assert.Equal(expense.Description, result.Description);
        Assert.Equal(expense.Date, result.Date);
        Assert.Equal(user.Id, result.UserId);
        
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenExpenseBelongsToAnotherUser()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;
        
        var owner = await TestData.CreateUserAsync(
            context,
            "expense-owner",
            "expense-owner@example.com");

        var otherUser = await TestData.CreateUserAsync(
            context,
            "other-user",
            "other-user@example.com");

        var expense = await TestData.CreateExpenseAsync(
            context,
            owner.Id,
            200,
            "Owner's expense");
        
        var expenseService = new ExpenseService(context);
        
        // Act
        var result = await expenseService.GetByIdAsync(expense.Id, otherUser.Id);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenExpenseDoesNotExist()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;
        
        var user = await TestData.CreateUserAsync(
            context,
            "missing-expense-user",
            "missing-expense@example.com");

        var expenseService = new ExpenseService(context);
        
        // Act
        var result = await expenseService.GetByIdAsync(999999, user.Id);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetExpensesByUserAsync_ShouldReturnUserExpenses_WhenUserHasExpenses()
    {
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;
        
        var user = await TestData.CreateUserAsync(
            context,
            "expense-user",
            "expense-user@example.com");

        var otherUser = await TestData.CreateUserAsync(
            context,
            "other-user",
            "other-user@example.com");
        
        var expense1 = await TestData.CreateExpenseAsync(
            context,
            user.Id,
            100,
            "Groceries",
            DateTime.UtcNow.AddDays(-2));
        
        var expense2 = await TestData.CreateExpenseAsync(
            context,
            user.Id,
            50,
            "Transport",
            DateTime.UtcNow.AddDays(-1));

        var expense3 = await TestData.CreateExpenseAsync(
            context,
            user.Id,
            200,
            "Utilities",
            DateTime.UtcNow);

        var otherUserExpense = await TestData.CreateExpenseAsync(
            context,
            otherUser.Id,
            500,
            "Other user's expense",
            DateTime.UtcNow);

        var expenseService = new ExpenseService(context);
        
        // Act
        var result = await expenseService.GetAllAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.UserId);
        Assert.NotNull(result.Expenses);
        Assert.Equal(3, result.Expenses.Count);

        Assert.Contains(result.Expenses, e =>
            e.Id == expense1.Id &&
            e.Amount == expense1.Amount &&
            e.Description == expense1.Description);

        Assert.Contains(result.Expenses, e =>
            e.Id == expense2.Id &&
            e.Amount == expense2.Amount &&
            e.Description == expense2.Description);

        Assert.Contains(result.Expenses, e =>
            e.Id == expense3.Id &&
            e.Amount == expense3.Amount &&
            e.Description == expense3.Description);

        Assert.DoesNotContain(result.Expenses, e =>
            e.Id == otherUserExpense.Id);
    }
    
    [Fact]
    public async Task GetExpensesByUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;

        var expenseService = new ExpenseService(context);
        var userId = Guid.NewGuid();
        
        // Act
        var result = await expenseService.GetAllAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Empty(result.Expenses);
    }
    
    [Fact]
    public async Task GetExpensesByUserAsync_ShouldReturnEmptyList_WhenUserHasNoExpenses()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync(); 
        var context = test.Context;
        
        var user = await TestData.CreateUserAsync(
            context,
            "no-expenses-user",
            "no-expenses@example.com");

        var expenseService = new ExpenseService(context);
        
        // Act
        var result = await expenseService.GetAllAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.UserId);
        Assert.NotNull(result.Expenses);
        Assert.Empty(result.Expenses);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateExpense_WhenExpenseBelongsToUser()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;
        
        var user = await TestData.CreateUserAsync(
            context,
            "update-test-user",
            "update-test@example.com");
        
        var expense = TestData.CreateExpense(
            user.Id,
            100,
            "Old description",
            DateTime.UtcNow.AddDays(-1));

        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var dto = new UpdateExpenseDto
        {
            Amount = 250,
            Description = "Updated description",
            Date = DateTime.UtcNow
        };

        var expenseService = new ExpenseService(context);
        
        // Act
        var result = await expenseService.UpdateAsync(expense.Id, dto, user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expense.Id, result.Id);
        Assert.Equal(dto.Amount, result.Amount);
        Assert.Equal(dto.Description, result.Description);
        Assert.Equal(dto.Date, result.Date);
        Assert.Equal(user.Id, result.UserId);

        var updatedExpense = await context.Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == expense.Id);

        Assert.NotNull(updatedExpense);
        Assert.Equal(dto.Amount, updatedExpense.Amount);
        Assert.Equal(dto.Description, updatedExpense.Description);
        Assert.Equal(dto.Date, updatedExpense.Date);
        Assert.Equal(user.Id, updatedExpense.UserId);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenExpenseDoesNotExist()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;
        
        var user = await TestData.CreateUserAsync(
            context,
            "missing-update-user",
            "missing-update@example.com");

        var dto = new UpdateExpenseDto
        {
            Amount = 250,
            Description = "Updated expense",
            Date = DateTime.UtcNow
        };

        var expenseService = new ExpenseService(context);
       
        // Act
        var result = await expenseService.UpdateAsync(999999, dto, user.Id);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenExpenseBelongsToAnotherUser()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;
        
        var owner = await TestData.CreateUserAsync(
            context,
            "update-owner",
            "update-owner@example.com");

        var otherUser = await TestData.CreateUserAsync(
            context,
            "other-update-user",
            "other-update@example.com");
        
        var expense = await TestData.CreateExpenseAsync(
            context,
            owner.Id,
            100,
            "Original expense",
            DateTime.UtcNow.AddDays(-1));

        var dto = new UpdateExpenseDto
        {
            Amount = 999,
            Description = "Unauthorized update",
            Date = DateTime.UtcNow
        };

        var expenseService = new ExpenseService(context);
        
        // Act
        var result = await expenseService.UpdateAsync(expense.Id, dto, otherUser.Id);

        // Assert
        Assert.Null(result);

        var unchangedExpense = await context.Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == expense.Id);

        Assert.NotNull(unchangedExpense);
        Assert.Equal(100, unchangedExpense.Amount);
        Assert.Equal("Original expense", unchangedExpense.Description);
        Assert.Equal(expense.Date, unchangedExpense.Date);
        Assert.Equal(owner.Id, unchangedExpense.UserId);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenDateIsMissing()
    {
        // Arrange
        await using var test = await TestContext.CreateAsync();
        var context = test.Context;
        
        var user = await TestData.CreateUserAsync(
            context,
            "missing-update-date-user",
            "missing-update-date@example.com");
        
        var expense = await TestData.CreateExpenseAsync(
            context,
            user.Id,
            100,
            "Original expense",
            DateTime.UtcNow.AddDays(-1));

        var dto = new UpdateExpenseDto
        {
            Amount = 500,
            Description = "Should not update",
            Date = null
        };

        var expenseService = new ExpenseService(context);
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => expenseService.UpdateAsync(expense.Id, dto, user.Id));

        Assert.Equal("Date is required.", exception.Message);

        var unchangedExpense = await context.Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == expense.Id);

        Assert.NotNull(unchangedExpense);
        Assert.Equal(100, unchangedExpense.Amount);
        Assert.Equal("Original expense", unchangedExpense.Description);
        Assert.Equal(expense.Date, unchangedExpense.Date);
        Assert.Equal(user.Id, unchangedExpense.UserId);
    }
    
    [Fact]
    public async Task GetAllAsync_ReturnsCategoryInformation()
    {
        await using var testContext = await TestContext.CreateAsync();

        var user = await TestData.CreateUserAsync(
            testContext.Context,
            "category-user",
            "category@example.com"
        );

        var category = new Category
        {
            Name = "Groceries",
            UserId = user.Id
        };

        testContext.Context.Categories.Add(category);
        await testContext.Context.SaveChangesAsync();

        var expense = await TestData.CreateExpenseAsync(
            testContext.Context,
            user.Id,
            250,
            "Weekly groceries"
        );

        expense.CategoryId = category.Id;
        await testContext.Context.SaveChangesAsync();

        var service = new ExpenseService(testContext.Context);

        var result = await service.GetAllAsync(user.Id);

        Assert.Single(result.Expenses);

        var returnedExpense = result.Expenses[0];

        Assert.Equal(expense.Id, returnedExpense.Id);
        Assert.Equal(category.Id, returnedExpense.CategoryId);
        Assert.Equal("Groceries", returnedExpense.CategoryName);
    }
    
    [Fact]
    public async Task GetAllAsync_ReturnsNullCategory_WhenExpenseHasNoCategory()
    {
        await using var testContext = await TestContext.CreateAsync();

        var user = await TestData.CreateUserAsync(
            testContext.Context,
            "no-category-user",
            "no-category@example.com"
        );

        var expense = await TestData.CreateExpenseAsync(
            testContext.Context,
            user.Id,
            100,
            "Cash expense"
        );

        var service = new ExpenseService(testContext.Context);

        var result = await service.GetAllAsync(user.Id);

        Assert.Single(result.Expenses);

        var returnedExpense = result.Expenses[0];

        Assert.Equal(expense.Id, returnedExpense.Id);
        Assert.Null(returnedExpense.CategoryId);
        Assert.Null(returnedExpense.CategoryName);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyExpensesInRange_WhenFromAndToAreGiven()
    {
        await using var testContext = await TestContext.CreateAsync();

        var user = await TestData.CreateUserAsync(
            testContext.Context,
            "range-user",
            "range@example.com"
        );

        var from = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.Zero);

        var lastDayOfSeptember = await TestData.CreateExpenseAsync(
            testContext.Context, user.Id, 10, "September", from.AddMinutes(-1));

        var firstMomentOfOctober = await TestData.CreateExpenseAsync(
            testContext.Context, user.Id, 20, "October start", from);

        var midOctober = await TestData.CreateExpenseAsync(
            testContext.Context, user.Id, 30, "October middle", from.AddDays(14));

        var firstMomentOfNovember = await TestData.CreateExpenseAsync(
            testContext.Context, user.Id, 40, "November", to);

        var service = new ExpenseService(testContext.Context);

        var result = await service.GetAllAsync(user.Id, from, to);

        Assert.Equal(2, result.Expenses.Count);
        Assert.Contains(result.Expenses, e => e.Id == firstMomentOfOctober.Id);
        Assert.Contains(result.Expenses, e => e.Id == midOctober.Id);
        Assert.DoesNotContain(result.Expenses, e => e.Id == lastDayOfSeptember.Id);
        Assert.DoesNotContain(result.Expenses, e => e.Id == firstMomentOfNovember.Id);
    }

    [Fact]
    public async Task GetAllAsync_UsesTheSameMoment_WhenRangeHasATimeZoneOffset()
    {
        await using var testContext = await TestContext.CreateAsync();

        var user = await TestData.CreateUserAsync(
            testContext.Context,
            "offset-user",
            "offset@example.com"
        );

        var expense = await TestData.CreateExpenseAsync(
            testContext.Context,
            user.Id,
            15,
            "Late night snack",
            new DateTimeOffset(2026, 9, 30, 23, 30, 0, TimeSpan.Zero));

        var from = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.FromHours(1));
        var to = new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.FromHours(1));

        var service = new ExpenseService(testContext.Context);

        var result = await service.GetAllAsync(user.Id, from, to);

        Assert.Single(result.Expenses);
        Assert.Equal(expense.Id, result.Expenses[0].Id);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllExpenses_WhenNoRangeIsGiven()
    {
        await using var testContext = await TestContext.CreateAsync();

        var user = await TestData.CreateUserAsync(
            testContext.Context,
            "no-range-user",
            "no-range@example.com"
        );

        await TestData.CreateExpenseAsync(
            testContext.Context, user.Id, 10, "Old", DateTimeOffset.UtcNow.AddYears(-1));

        await TestData.CreateExpenseAsync(
            testContext.Context, user.Id, 20, "Recent", DateTimeOffset.UtcNow);

        var service = new ExpenseService(testContext.Context);

        var result = await service.GetAllAsync(user.Id);

        Assert.Equal(2, result.Expenses.Count);
    }
}
