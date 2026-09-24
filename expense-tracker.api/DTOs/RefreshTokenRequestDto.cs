using System.ComponentModel.DataAnnotations;

namespace expense_tracker.api.DTOs;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}