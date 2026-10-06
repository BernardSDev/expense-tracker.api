namespace expense_tracker.api.Security.CurrentUser;

public interface ICurrentUserService
{
    Guid UserId { get; }
}