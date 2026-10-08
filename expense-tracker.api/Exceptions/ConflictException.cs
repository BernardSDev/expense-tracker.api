namespace expense_tracker.api.Exceptions;

public class ConflictException(string message) : Exception(message)
{
}