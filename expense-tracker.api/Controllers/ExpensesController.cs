using expense_tracker.api.DTOs;
using expense_tracker.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace expense_tracker.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController(IExpenseService expenseService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseDto dto)
    {
        var expense = await expenseService.CreateAsync(dto);
       
        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var expense = await expenseService.GetByIdAsync(id);

        if (expense is null)
        {
            return NotFound();
        }
        
        return Ok(expense);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateExpenseDto dto)
    {
        var result = await expenseService.UpdateAsync(id, dto);

        if (result is null)
        {
            return NotFound();
        }
        
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await expenseService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}