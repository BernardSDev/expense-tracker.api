using expense_tracker.api.Models;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.api.Data;

public class AppDbContext : DbContext
{
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<User> Users { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}