using expense_tracker.api.Data;
using expense_tracker.api.Models;
using expense_tracker.api.Security;
using expense_tracker.api.Services;
using expense_tracker.api.Services.Auth;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        
        services.AddScoped<Microsoft.AspNetCore.Identity.PasswordHasher<User>>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        services.AddHttpContextAccessor();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();
        
        return services;
    }
    
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "DefaultConnection")
    {
        services.AddDbContext<AppDbContext>(
            options => options.UseNpgsql(
                configuration.GetConnectionString(connectionStringName)));

        return services;
    }
}