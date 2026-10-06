using System.ComponentModel.DataAnnotations;

namespace expense_tracker.api.DTOs.Categories;

public class CreateExpenseDto
{
    [Range(0.01, 10_000_000)]
    public decimal Amount { get; set; }
    
    [Required]
    [MaxLength(250)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public DateTimeOffset? Date { get; set; }
    
    public int? CategoryId { get; set; }
}