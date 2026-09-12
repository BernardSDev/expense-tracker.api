using expense_tracker.api.DTOs;
using expense_tracker.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace expense_tracker.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseDto dto)
    {
        var expense = await _expenseService.CreateAsync(dto);
       
        return CreatedAtAction(
            nameof(GetById), 
            new { id = expense.Id }, 
            expense
            );
        
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var expense = await _expenseService.GetByIdAsync(id);

        if (expense is null)
        {
            return NotFound();
        }
        
        return Ok(expense);
    }
}