namespace expense_tracker.api.DTOs;

public class LogoutRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}