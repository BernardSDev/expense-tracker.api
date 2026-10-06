namespace expense_tracker.api.DTOs.Expenses;

public class ExpenseResponseDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset Date { get; set; }
    public Guid UserId { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
}