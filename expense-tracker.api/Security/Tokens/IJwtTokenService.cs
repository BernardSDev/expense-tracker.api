using expense_tracker.api.Models;

namespace expense_tracker.api.Security.Tokens;

public interface IJwtTokenService
{
    string CreateAccessToken(User user, int sessionId);
}