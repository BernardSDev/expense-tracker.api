using expense_tracker.api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace expense_tracker.api.Extensions;

public static class ControllerExtensions
{
    public static IActionResult NotFoundError(this ControllerBase controller, string details)
    {
        return controller.NotFound(new ErrorResponseDto
        {
            Status = StatusCodes.Status404NotFound,
            Message = "Not Found",
            Details = "Expense not found." 
        });
    }
}