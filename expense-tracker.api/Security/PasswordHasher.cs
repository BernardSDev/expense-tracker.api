using expense_tracker.api.Models;
using Microsoft.AspNetCore.Identity;

namespace expense_tracker.api.Security;

public class PasswordHasher(PasswordHasher<User> passwordHasher) : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return passwordHasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var result = passwordHasher.VerifyHashedPassword(null!, passwordHash, password);
        return result != PasswordVerificationResult.Failed;
    }
}