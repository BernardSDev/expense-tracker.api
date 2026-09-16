using System.ComponentModel.DataAnnotations;

namespace expense_tracker.api.DTOs;

public class UpdateExpenseDto
{
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
    
    [Required]
    [MaxLength(250)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public DateTimeOffset Date { get; set; }
}