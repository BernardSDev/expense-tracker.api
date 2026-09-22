namespace expense_tracker.api.Security;

public interface IRefreshTokenHasher
{
    string Hash(string token);
}