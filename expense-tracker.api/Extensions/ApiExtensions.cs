using expense_tracker.api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace expense_tracker.api.Extensions;

public static class ApiExtensions
{
    public static IServiceCollection AddApiConfiguration(
        this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(entry => entry.Value?.Errors.Count > 0)
                        .ToDictionary(
                            entry => entry.Key,
                            entry => entry.Value!.Errors
                                .Select(error => error.ErrorMessage)
                                .ToArray());

                    return new BadRequestObjectResult(new ErrorResponseDto
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Validation failed.",
                        Errors = errors
                    });
                };
            });

        return services;
    }
}