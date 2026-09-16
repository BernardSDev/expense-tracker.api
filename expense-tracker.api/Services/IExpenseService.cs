using expense_tracker.api.DTOs;

namespace expense_tracker.api.Services;

public interface IExpenseService
{
    Task<ExpenseResponseDto> CreateAsync(CreateExpenseDto dto);
    Task<ExpenseResponseDto?> GetByIdAsync(int id);
    Task<UserExpensesResponseDto?> GetExpensesByUserAsync(int userId);
    
    Task<ExpenseResponseDto?> UpdateAsync(int id, UpdateExpenseDto dto);
}