using expense_tracker.api.Data;
using expense_tracker.api.Models;

namespace expense_tracker.api.Tests.Infrastructure;

public static class TestData
{
    public static User CreateUser(
        string username = "test-user",
        string email = "test@example.com")
    {
        return new User
        {
            Username = username,
            Email = email,
            PasswordHash = "test-password",
            FirstName = "Test",
            LastName = "User"
        };
    }

    public static Expense CreateExpense(
        Guid userId,
        decimal amount = 100,
        string description = "Test expense",
        DateTimeOffset? date = null)
    {
        return new Expense
        {
            Amount = amount,
            Description = description,
            Date = date ?? DateTimeOffset.UtcNow,
            UserId = userId
        };
    }

    public static async Task<User> CreateUserAsync(
        AppDbContext context,
        string username = "test-user",
        string email = "test@example.com")
    {
        var user = CreateUser(username, email);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    public static async Task<Expense> CreateExpenseAsync(
        AppDbContext context,
        Guid userId,
        decimal amount = 100,
        string description = "Test expense",
        DateTimeOffset? date = null)
    {
        var expense = CreateExpense(
            userId,
            amount,
            description,
            date);

        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        return expense;
    }
}