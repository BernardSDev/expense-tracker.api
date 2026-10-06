namespace expense_tracker.api.Models;

public class User
{
    public Guid Id { get; set; }
    
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}