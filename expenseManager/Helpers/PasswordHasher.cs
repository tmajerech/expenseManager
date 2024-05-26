using expenseManager.Models;

namespace expenseManager.Helpers;
using Microsoft.AspNetCore.Identity;


public static class PasswordHasher
{
    private static readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    public static string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(null, password);
    }

    public static bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}