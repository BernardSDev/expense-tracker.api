using expense_tracker.api.DTOs;

namespace expense_tracker.api.Services;

public interface IExpenseService
{
    Task<ExpenseResponseDto> CreateAsync(CreateExpenseDto dto, Guid userId); 
    Task<ExpenseResponseDto?> GetByIdAsync(int id, Guid userId);
    Task<UserExpensesResponseDto?> GetExpensesByUserAsync(Guid userId);
    
    Task<ExpenseResponseDto?> UpdateAsync(int id, UpdateExpenseDto dto, Guid userId);
    
    Task<bool> DeleteAsync(int id,  Guid userId);
}