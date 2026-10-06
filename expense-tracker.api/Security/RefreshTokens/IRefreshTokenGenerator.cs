namespace expense_tracker.api.Security.RefreshTokens;

public interface IRefreshTokenGenerator
{
    string GenerateRefreshToken();
}