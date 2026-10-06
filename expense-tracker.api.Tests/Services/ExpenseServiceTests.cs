using expense_tracker.api.DTOs;
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
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnExpense_WhenExpenseBelongsToUser()
    {
        // Arrange
        await using var context = TestDatabase.CreateContext();
        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var user = new User
        {
            Username = "get-by-id-user",
            Email = "get-by-id@example.com",
            PasswordHash = "test-password",
            FirstName = "Get",
            LastName = "Test"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        var expense = new Expense
        {
            Amount = 150,
            Description = "Groceries",
            Date = DateTime.UtcNow,
            UserId = user.Id
        };

        context.Expenses.Add(expense);
        await context.SaveChangesAsync();
        
        var expenseService = new ExpenseService(context);

        try
        {
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
        finally
        {
            await transaction.RollbackAsync();
        }
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenExpenseBelongsToAnotherUser()
    {
        // Arrange
        await using var context = TestDatabase.CreateContext();
        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var owner = new User
        {
            Username = "expense-owner",
            Email = "expense-owner@example.com",
            PasswordHash = "test-password",
            FirstName = "Expense",
            LastName = "Owner"
        };

        var otherUser = new User
        {
            Username = "other-user",
            Email = "other-user@example.com",
            PasswordHash = "test-password",
            FirstName = "Other",
            LastName = "User"
        };

        context.Users.AddRange(owner, otherUser);
        await context.SaveChangesAsync();
        
        var expense = new Expense
        {
            Amount = 200,
            Description = "Owner's expense",
            Date = DateTime.UtcNow,
            UserId = owner.Id
        };

        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var expenseService = new ExpenseService(context);
        
        try
        {
            // Act
            var result = await expenseService.GetByIdAsync(
                expense.Id,
                otherUser.Id);

            // Assert
            Assert.Null(result);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenExpenseDoesNotExist()
    {
        // Arrange
        await using var context = TestDatabase.CreateContext();
        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var user = new User
        {
            Username = "missing-expense-user",
            Email = "missing-expense@example.com",
            PasswordHash = "test-password",
            FirstName = "Missing",
            LastName = "Expense"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var expenseService = new ExpenseService(context);

        try
        {
            // Act
            var result = await expenseService.GetByIdAsync(999999, user.Id);

            // Assert
            Assert.Null(result);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }
    
    [Fact]
    public async Task GetExpensesByUserAsync_ShouldReturnUserExpenses_WhenUserHasExpenses()
    {
        // Arrange
        await using var context = TestDatabase.CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();

        var user = new User
        {
            Username = "expenses-user",
            Email = "expenses-user@example.com",
            PasswordHash = "test-password",
            FirstName = "Expenses",
            LastName = "User"
        };

        var otherUser = new User
        {
            Username = "other-expenses-user",
            Email = "other-expenses-user@example.com",
            PasswordHash = "test-password",
            FirstName = "Other",
            LastName = "User"
        };

        context.Users.AddRange(user, otherUser);
        await context.SaveChangesAsync();

        var expense1 = new Expense
        {
            Amount = 100,
            Description = "Groceries",
            Date = DateTime.UtcNow.AddDays(-2),
            UserId = user.Id
        };

        var expense2 = new Expense
        {
            Amount = 50,
            Description = "Transport",
            Date = DateTime.UtcNow.AddDays(-1),
            UserId = user.Id
        };

        var expense3 = new Expense
        {
            Amount = 200,
            Description = "Utilities",
            Date = DateTime.UtcNow,
            UserId = user.Id
        };
        
        var otherUserExpense = new Expense
        {
            Amount = 500,
            Description = "Other user's expense",
            Date = DateTime.UtcNow,
            UserId = otherUser.Id
        };
        
        context.Expenses.AddRange(
            expense1,
            expense2,
            expense3,
            otherUserExpense);
        
        await context.SaveChangesAsync();

        var expenseService = new ExpenseService(context);
        
        try
        {
            // Act
            var result = await expenseService.GetExpensesByUserAsync(user.Id);

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
        
        finally
        {
            await transaction.RollbackAsync();
        }
    }
    
    [Fact]
    public async Task GetExpensesByUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        await using var context = TestDatabase.CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();

        var expenseService = new ExpenseService(context);
        var userId = Guid.NewGuid();

        try
        {
            // Act
            var result = await expenseService.GetExpensesByUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Empty(result.Expenses);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }
    
    [Fact]
    public async Task GetExpensesByUserAsync_ShouldReturnEmptyList_WhenUserHasNoExpenses()
    {
        // Arrange
        await using var context = TestDatabase.CreateContext();
        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var user = new User
        {
            Username = "no-expenses-user",
            Email = "no-expenses@example.com",
            PasswordHash = "test-password",
            FirstName = "No",
            LastName = "Expenses"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var expenseService = new ExpenseService(context);

        try
        {
            // Act
            var result = await expenseService.GetExpensesByUserAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.UserId);
            Assert.NotNull(result.Expenses);
            Assert.Empty(result.Expenses);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateExpense_WhenExpenseBelongsToUser()
    {
        // Arrange
        await using var context = TestDatabase.CreateContext();
        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var user = new User
        {
            Username = "update-user",
            Email = "update-user@example.com",
            PasswordHash = "test-password",
            FirstName = "Update",
            LastName = "User"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var expense = new Expense
        {
            Amount = 100,
            Description = "Old description",
            Date = DateTime.UtcNow.AddDays(-1),
            UserId = user.Id
        };

        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var dto = new UpdateExpenseDto
        {
            Amount = 250,
            Description = "Updated description",
            Date = DateTime.UtcNow
        };

        var expenseService = new ExpenseService(context);

        try
        {
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
        finally
        {
            await transaction.RollbackAsync();
        }
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenExpenseDoesNotExist()
    {
        // Arrange
        await using var context = TestDatabase.CreateContext();
        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var user = new User
        {
            Username = "missing-update-user",
            Email = "missing-update@example.com",
            PasswordHash = "test-password",
            FirstName = "Missing",
            LastName = "Update"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var dto = new UpdateExpenseDto
        {
            Amount = 250,
            Description = "Updated expense",
            Date = DateTime.UtcNow
        };

        var expenseService = new ExpenseService(context);

        try
        {
            // Act
            var result = await expenseService.UpdateAsync(
                999999,
                dto,
                user.Id);

            // Assert
            Assert.Null(result);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenExpenseBelongsToAnotherUser()
    {
        // Arrange
        await using var context = TestDatabase.CreateContext();
        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var owner = new User
        {
            Username = "update-owner",
            Email = "update-owner@example.com",
            PasswordHash = "test-password",
            FirstName = "Update",
            LastName = "Owner"
        };

        var otherUser = new User
        {
            Username = "other-update-user",
            Email = "other-update@example.com",
            PasswordHash = "test-password",
            FirstName = "Other",
            LastName = "User"
        };

        context.Users.AddRange(owner, otherUser);
        await context.SaveChangesAsync();

        var expense = new Expense
        {
            Amount = 100,
            Description = "Original expense",
            Date = DateTime.UtcNow.AddDays(-1),
            UserId = owner.Id
        };

        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var dto = new UpdateExpenseDto
        {
            Amount = 999,
            Description = "Unauthorized update",
            Date = DateTime.UtcNow
        };

        var expenseService = new ExpenseService(context);

        try
        {
            // Act
            var result = await expenseService.UpdateAsync(
                expense.Id,
                dto,
                otherUser.Id);

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
        finally
        {
            await transaction.RollbackAsync();
        }
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenDateIsMissing()
    {
        // Arrange
        await using var context = TestDatabase.CreateContext();
        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var user = new User
        {
            Username = "missing-update-date-user",
            Email = "missing-update-date@example.com",
            PasswordHash = "test-password",
            FirstName = "Missing",
            LastName = "Date"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var expense = new Expense
        {
            Amount = 100,
            Description = "Original expense",
            Date = DateTime.UtcNow.AddDays(-1),
            UserId = user.Id
        };

        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var dto = new UpdateExpenseDto
        {
            Amount = 500,
            Description = "Should not update",
            Date = null
        };

        var expenseService = new ExpenseService(context);

        try
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => expenseService.UpdateAsync(
                    expense.Id,
                    dto,
                    user.Id));

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
        finally
        {
            await transaction.RollbackAsync();
        }
    }
}