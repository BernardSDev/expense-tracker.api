using System.Security.Claims;
using expense_tracker.api.Data;
using expense_tracker.api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace expense_tracker.api.Middleware;

public class SessionValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        AppDbContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var sessionIdClaim = context.User.FindFirst(ClaimTypes.Sid);

            if (sessionIdClaim is null ||
                !int.TryParse(sessionIdClaim.Value, out var sessionId))
            {
                throw new UnauthorizedException("Invalid session.");
            }

            var session = await dbContext.RefreshTokens
                .FirstOrDefaultAsync(token => token.Id == sessionId);

            if (session is null)
            {
                throw new UnauthorizedException("Invalid session.");
            }

            if (session.RevokedAt is not null)
            {
                throw new UnauthorizedException("Session has been logged out.");
            }

            if (session.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                throw new UnauthorizedException("Session has expired.");
            }
        }

        await next(context);
    }
}