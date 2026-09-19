using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace expense_tracker.api.Security;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var userIdClaim = httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Authenticated user ID is missing or invalid.");
            }

            return userId;
        }
    }
}