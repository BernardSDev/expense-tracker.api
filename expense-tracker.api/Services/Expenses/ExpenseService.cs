using System.ComponentModel;
using expense_tracker.api.Data;
using expense_tracker.api.DTOs;
using expense_tracker.api.DTOs.Categories;
using expense_tracker.api.DTOs.Expenses;
using expense_tracker.api.Exceptions;
using expense_tracker.api.Models;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.api.Services;

public class ExpenseService(AppDbContext context) : IExpenseService
{
    public async Task<ExpenseResponseDto> CreateAsync(CreateExpenseDto dto, Guid userId)
    {
        string? categoryName = null;

        if (dto.CategoryId.HasValue)
        {
            categoryName = await context.Categories
                .Where(category =>
                    category.Id == dto.CategoryId.Value &&
                    category.UserId == userId)
                .Select(category => category.Name)
                .FirstOrDefaultAsync();

            if (categoryName is null)
                throw new NotFoundException("Category not found.");
        }

        var expense = new Expense
        {
            Amount = dto.Amount,
            Description = dto.Description,
            Date = dto.Date ?? throw new InvalidOperationException("Date is required."),
            UserId = userId,
            CategoryId = dto.CategoryId
        };

        context.Expenses.Add(expense);

        await context.SaveChangesAsync();

        return new ExpenseResponseDto()
        {
            Id = expense.Id,
            Amount = expense.Amount,
            Description = expense.Description,
            Date = expense.Date,
            UserId = expense.UserId,
            CategoryId = expense.CategoryId,
            CategoryName = categoryName
        };
    }
    
    public async Task<ExpenseResponseDto?> GetByIdAsync(int id, Guid userId)
    {
        var expense = await context.Expenses.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

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
    
    public async Task<UserExpensesResponseDto> GetExpensesByUserAsync(Guid userId)
    {
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

    public async Task<ExpenseResponseDto?> UpdateAsync(int id, UpdateExpenseDto dto, Guid userId)
    {
        var expense = await context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        
        if (expense is null)
        {
            return null;
        }
        
        string? categoryName = null;

        if (dto.CategoryId.HasValue)
        {
            categoryName = await context.Categories.Where(category =>
                category.Id == dto.CategoryId.Value && 
                category.UserId == userId)
                .Select(category => category.Name)
                .FirstOrDefaultAsync();
            
            if (categoryName is null) 
                throw new NotFoundException("Category not found.");
        }

        expense.Amount = dto.Amount;
        expense.Description = dto.Description;
        expense.Date = dto.Date ?? throw new InvalidOperationException("Date is required.");
        expense.CategoryId = dto.CategoryId;
        
        await context.SaveChangesAsync();

        return new ExpenseResponseDto
        {
            Id = expense.Id,
            Amount = expense.Amount,
            Description = expense.Description,
            Date = expense.Date,
            UserId = expense.UserId,
            CategoryId = expense.CategoryId,
            CategoryName = categoryName
        };
    }

    public async Task<bool> DeleteAsync(int id,  Guid userId)
    {
        var expense = await context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        
        if (expense is null)
        {
            return false;
        }
        
        context.Expenses.Remove(expense);
        
        await context.SaveChangesAsync();
        
        return true;
    }
}