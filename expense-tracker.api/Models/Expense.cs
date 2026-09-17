namespace expense_tracker.api.Models;

public class Expense
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTimeOffset Date { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}