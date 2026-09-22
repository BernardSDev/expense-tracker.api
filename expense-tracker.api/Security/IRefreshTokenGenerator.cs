namespace expense_tracker.api.Security;

public interface IRefreshTokenGenerator
{
    string GenerateRefreshToken();
}