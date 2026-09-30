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
        await using var transaction = await context.Database.BeginTransactionAsync();

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
            var result = await expenseService.DeleteAsync(expense.Id, user.Id);

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
}