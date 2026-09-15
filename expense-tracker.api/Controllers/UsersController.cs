using expense_tracker.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace expense_tracker.api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UsersController(IExpenseService expenseService) : ControllerBase
{
    [HttpGet("{userId}/Expenses")]
    
    public async Task<IActionResult> GetExpensesByUserAsync(int userId)
    {
        var result = await expenseService.GetExpensesByUserAsync(userId);
        
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}