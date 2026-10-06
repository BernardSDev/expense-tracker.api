using System.ComponentModel.DataAnnotations;

namespace expense_tracker.api.DTOs.Auth;

public class LoginDto
{
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}