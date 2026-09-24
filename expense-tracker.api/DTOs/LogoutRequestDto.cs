using System.ComponentModel.DataAnnotations;

namespace expense_tracker.api.DTOs;

public class LogoutRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}