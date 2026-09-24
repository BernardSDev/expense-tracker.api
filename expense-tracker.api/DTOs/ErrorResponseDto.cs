namespace expense_tracker.api.DTOs;

public class ErrorResponseDto
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
}