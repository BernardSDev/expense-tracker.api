using expense_tracker.api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace expense_tracker.api.Tests.Infrastructure;

public static class TestDatabase
{
    public static AppDbContext CreateContext()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TestAssemblyMarker>()
            .Build();

        var connectionString =
            configuration.GetConnectionString("TestConnection");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}