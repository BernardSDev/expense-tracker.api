using expense_tracker.api.DTOs;
using expense_tracker.api.DTOs.Users;
using expense_tracker.api.Security;
using expense_tracker.api.Security.CurrentUser;
using expense_tracker.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace expense_tracker.api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UsersController(
    IExpenseService expenseService, 
    ICurrentUserService currentUserService,
    IUserService userService
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
    
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var profile = await userService.GetProfileAsync();

        return Ok(profile);
    }
    
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var profile = await userService.UpdateProfileAsync(dto);

        return Ok(profile);
    }
}