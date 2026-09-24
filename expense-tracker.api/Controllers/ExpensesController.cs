using expense_tracker.api.DTOs;
using expense_tracker.api.Security;
using expense_tracker.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace expense_tracker.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController(
    IExpenseService expenseService, 
    ICurrentUserService currentUserService
) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseDto dto)
    {
        var userId = currentUserService.UserId;
        
        var expense = await expenseService.CreateAsync(dto,  userId);
       
        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = currentUserService.UserId;
        
        var expense = await expenseService.GetByIdAsync(id, userId);

        if (expense is null)
        {
            return NotFound( new ErrorResponseDto
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Not Found",
                Details = "Expense not found."
            });
        }
        
        return Ok(expense);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateExpenseDto dto)
    {
        var userId = currentUserService.UserId;
        
        var result = await expenseService.UpdateAsync(id, dto, userId);

        if (result is null)
        {
            return NotFound(
                new ErrorResponseDto
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Not Found",
                    Details = "Expense not found." 
                });
        }
        
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = currentUserService.UserId;
        
        var deleted = await expenseService.DeleteAsync(id,  userId);

        if (!deleted)
        {
            return NotFound(
                
                new ErrorResponseDto
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Not Found",
                    Details = "Expense not found." 
                });
        }
        
        return NoContent();
    }
}