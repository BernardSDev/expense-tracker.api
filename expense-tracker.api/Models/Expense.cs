namespace expense_tracker.api.Models;

public class Expense
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int UserId { get; set; }                                         //[See 1]
    public User User { get; set; } = null!;                                 //[See 2]
}

/*
 * [See 1] Foreign-key property
 * Expense.UserId → stores the Id of the User this Expense belongs to.
 * Think: "Which User does this Expense belong to?"
 *
 * [See 2] Navigation property
 * Expense.User → gives access to the related User object.
 * Think: "Let me navigate from this Expense to its User."
 *
 * Relationship:
 *
 * Expense.UserId → identifies the User
 * Expense.User   → navigates to the User
 *
 * User.Id        → Primary Key
 * User.Expenses  → navigates to the User's Expenses
 */