using expense_tracker.api.Data;
using expense_tracker.api.DTOs;
using expense_tracker.api.Models;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.api.Services;

public class ExpenseService(AppDbContext context) : IExpenseService
{
    public async Task<ExpenseResponseDto> CreateAsync(CreateExpenseDto dto)
    {
        var expense = new Expense
        {
            Amount = dto.Amount,
            Description = dto.Description,
            Date = dto.Date,
            UserId = dto.UserId,
        };

        context.Expenses.Add(expense);

        await context.SaveChangesAsync();

        return new ExpenseResponseDto()
        {
            Id = expense.Id,
            Amount = expense.Amount,
            Description = expense.Description,
            Date = expense.Date,
            UserId = expense.UserId
        };
    }
    
    public async Task<ExpenseResponseDto?> GetByIdAsync(int id)
    {
        var expense = await context.Expenses.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

        if (expense is null)
        {
            return null;
        }
        
        return new ExpenseResponseDto
        {
            Id = expense.Id,
            Amount = expense.Amount,
            Description = expense.Description,
            Date = expense.Date,
            UserId = expense.UserId
        };
    }
    
    public async Task<UserExpensesResponseDto?> GetExpensesByUserAsync(int userId)
    {
        bool userExists = await context.Users.AnyAsync(u => u.Id == userId);

        if (!userExists)
        {
            return null;
        }

        var expenses = await context.Expenses
            .Where(e => e.UserId == userId)
            .Select(e => new ExpenseResponseDto
            {
                Id = e.Id,
                Amount = e.Amount,
                Description = e.Description,
                Date = e.Date,
                UserId = e.UserId
            })
            .ToListAsync();

        return new UserExpensesResponseDto
        {
            UserId = userId,
            Expenses = expenses
        };
    }

    public async Task<ExpenseResponseDto?> UpdateAsync(int id, UpdateExpenseDto dto)
    {
        var expense = await context.Expenses.FindAsync(id);
        
        if (expense is null)
        {
            return null;
        }

        expense.Amount = dto.Amount;
        expense.Description = dto.Description;
        expense.Date = dto.Date;
        
        await context.SaveChangesAsync();

        return new ExpenseResponseDto
        {
            Id = expense.Id,
            Amount = expense.Amount,
            Description = expense.Description,
            Date = expense.Date,
            UserId = expense.UserId
        };
    }
}