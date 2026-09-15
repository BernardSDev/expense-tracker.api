namespace expense_tracker.api.DTOs;

public class UserExpensesResponseDto
{
    public int UserId { get; set; }
    public List<ExpenseResponseDto> Expenses { get; set; } = [];
}