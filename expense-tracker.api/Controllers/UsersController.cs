using expense_tracker.api.Security;
using expense_tracker.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace expense_tracker.api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UsersController(
    IExpenseService expenseService, 
    ICurrentUserService currentUserService
) : ControllerBase
{
    [HttpGet("Expenses")]
    
    public async Task<IActionResult> GetMyExpenses()
    {
        var userId = currentUserService.UserId;
        
        var result = await expenseService.GetExpensesByUserAsync(userId);
        
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}