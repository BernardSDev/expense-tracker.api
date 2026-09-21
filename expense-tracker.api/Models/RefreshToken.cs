namespace expense_tracker.api.Models;

public class RefreshToken
{
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string TokenHash { get; set; } = string.Empty;
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset ExpiresAt { get; set; }
    
    public DateTimeOffset? RevokedAt { get; set; }
    
    public User User { get; set; } = null!;
}