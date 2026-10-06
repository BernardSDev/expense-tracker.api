namespace expense_tracker.api.Security.RefreshTokens;

public interface IRefreshTokenHasher
{
    string Hash(string token);
}