using expense_tracker.api.DTOs;
using expense_tracker.api.Exceptions;

namespace expense_tracker.api.Middleware;

public class GlobalExceptionHandlingMiddleware(
    RequestDelegate next, 
    ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var status = exception is UnauthorizedException 
                ? StatusCodes.Status401Unauthorized 
                : StatusCodes.Status500InternalServerError;

            if (status == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(exception, "Unhandled exception occurred while processing the request.");
            }
            
            var errorResponse = new ErrorResponseDto
            {
                Status = status,
                Message = status == StatusCodes.Status401Unauthorized ? "Unauthorized" : "Internal server error",
                Details = status == StatusCodes.Status401Unauthorized ? exception.Message : "An unhandled error occurred."
            };

            context.Response.StatusCode = errorResponse.Status;
            
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}