using expense_tracker.api.DTOs;
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
        await using var context = TestDatabase.CreateContext();
        var expenseService = new ExpenseService(context);

        // Act
        var result = await expenseService.DeleteAsync(
            999999,
            Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_AndDeleteExpense_WhenExpenseExists()
    {
        // Arrange
        await using var context = TestDatabase.CreateContext();
        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "test-password",
            FirstName = "Test",
            LastName = "User"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var expense = new Expense
        {
            Amount = 100,
            Description = "Test expense",
            Date = DateTime.UtcNow,
            UserId = user.Id
        };

        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var expenseService = new ExpenseService(context);

        try
        {
            // Act
            var result = await expenseService.DeleteAsync(
                expense.Id,
                user.Id);

            // Assert
            Assert.True(result);

            var deletedExpense = await context.Expenses
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == expense.Id);

            Assert.Null(deletedExpense);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }
    
    [Fact]
    public async Task CreateAsync_ShouldCreateExpense_WhenDataIsValid()
    {
        // Arrange
        await using var context = TestDatabase.CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();

        var user = new User
        {
            Username = "create-test-user",
            Email = "create-test@example.com",
            PasswordHash = "test-password",
            FirstName = "Create",
            LastName = "Test"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        var dto = new CreateExpenseDto
        {
            Amount = 250,
            Description = "Lunch",
            Date = DateTime.UtcNow
        };
        
        var expenseService = new ExpenseService(context);

        try
        {
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
        finally
        {
            await transaction.RollbackAsync();
        }
    }
    
    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenDateIsMissing()
    {
        // Arrange
        await using var context = TestDatabase.CreateContext();
        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var user = new User
        {
            Username = "missing-date-user",
            Email = "missing-date@example.com",
            PasswordHash = "test-password",
            FirstName = "Missing",
            LastName = "Date"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var dto = new CreateExpenseDto
        {
            Amount = 100,
            Description = "Missing date",
            Date = null
        };

        var expenseService = new ExpenseService(context);
        
        try
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => expenseService.CreateAsync(dto, user.Id));

            Assert.Equal("Date is required.", exception.Message);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }
}