using expense_tracker.api.Models;

namespace expense_tracker.api.Security;

public interface IJwtTokenService
{
    string CreateAccessToken(User user);
}