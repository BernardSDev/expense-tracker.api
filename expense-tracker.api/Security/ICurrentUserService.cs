namespace expense_tracker.api.Security;

public interface ICurrentUserService
{
    Guid UserId { get; }
}