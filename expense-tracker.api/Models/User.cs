namespace expense_tracker.api.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}