using expense_tracker.api.Data;
using expense_tracker.api.DTOs;
using expense_tracker.api.Models;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.api.Services;

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _context;

    public ExpenseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseResponseDto> CreateAsync(CreateExpenseDto dto)
    {
        var expense = new Expense
        {
            Amount = dto.Amount,
            Description = dto.Description,
            Date = dto.Date,
            UserId = dto.UserId,
        };

        _context.Expenses.Add(expense);

        await _context.SaveChangesAsync();

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
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);

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
}