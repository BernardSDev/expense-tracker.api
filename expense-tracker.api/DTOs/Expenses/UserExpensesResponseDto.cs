namespace expense_tracker.api.DTOs.Expenses;

public class UserExpensesResponseDto
{
    public Guid UserId { get; set; }
    public List<ExpenseResponseDto> Expenses { get; set; } = [];
}