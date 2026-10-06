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
            var status = exception switch
            {
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            if (status == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(exception, "Unhandled exception occurred while processing the request.");
            }
            
            var errorResponse = new ErrorResponseDto
            {
                Status = status,
                Message = status switch
                {
                    StatusCodes.Status401Unauthorized => "Unauthorized",
                    StatusCodes.Status404NotFound => "Not Found",
                    _ => "Internal server error"
                },
                Details = status == StatusCodes.Status500InternalServerError ? "An unhandled error occurred." : exception.Message
            };

            context.Response.StatusCode = errorResponse.Status;
            
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}