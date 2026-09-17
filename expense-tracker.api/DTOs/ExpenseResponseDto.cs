namespace expense_tracker.api.DTOs;

public class ExpenseResponseDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset Date { get; set; }
    public Guid UserId { get; set; }
}