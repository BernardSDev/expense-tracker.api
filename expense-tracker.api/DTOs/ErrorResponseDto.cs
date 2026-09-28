using System.Text.Json.Serialization;

namespace expense_tracker.api.DTOs;

public class ErrorResponseDto
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Details { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string[]>? Errors { get; set; }
}