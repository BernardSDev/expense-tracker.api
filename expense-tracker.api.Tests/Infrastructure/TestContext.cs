using expense_tracker.api.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace expense_tracker.api.Tests.Infrastructure;

public class TestContext(AppDbContext context, IDbContextTransaction transaction) : IAsyncDisposable
{ 
    public AppDbContext Context { get; } = context;
    
    private readonly IDbContextTransaction _transaction = transaction;

    public static async Task<TestContext> CreateAsync()
    {
        var context = TestDatabase.CreateContext();

        var transaction = await context.Database.BeginTransactionAsync();
        
        return new TestContext(context, transaction);
    }
    
    public async ValueTask DisposeAsync()
    {
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        await Context.DisposeAsync();
    }
}