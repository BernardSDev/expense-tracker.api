namespace expense_tracker.api.Constants;

public static class ErrorMessages
{
    public const string NotFound = "Not Found";

    public const string ExpenseNotFound = "Expense not found.";

    public const string CategoryNotFound = "Category not found.";

    public const string CategoryAlreadyExists = "A category with this name already exists.";

    public const string CategoryInUse = "Category cannot be deleted because it is being used by expenses.";
}