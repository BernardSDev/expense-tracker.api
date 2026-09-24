namespace expense_tracker.api.Exceptions;

public class UnauthorizedException : System.Exception
{
    public UnauthorizedException(string message) : base(message)
    {
        
    }
}