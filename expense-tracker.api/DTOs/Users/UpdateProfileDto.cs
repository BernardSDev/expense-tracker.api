using System.ComponentModel.DataAnnotations;

namespace expense_tracker.api.DTOs.Users;

public class UpdateProfileDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
}