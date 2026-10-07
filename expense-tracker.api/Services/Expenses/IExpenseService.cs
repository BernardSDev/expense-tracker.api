using expense_tracker.api.DTOs;
using expense_tracker.api.DTOs.Categories;
using expense_tracker.api.DTOs.Expenses;

namespace expense_tracker.api.Services;

public interface IExpenseService
{
    Task<ExpenseResponseDto> CreateAsync(CreateExpenseDto dto, Guid userId); 
    Task<ExpenseResponseDto?> GetByIdAsync(int id, Guid userId);
    Task<UserExpensesResponseDto> GetAllAsync(Guid userId);
    
    Task<ExpenseResponseDto?> UpdateAsync(int id, UpdateExpenseDto dto, Guid userId);
    
    Task<bool> DeleteAsync(int id,  Guid userId);
}