namespace WebApplication1.Helper;

public static class PasswordHelper
{
    public static string GeneratePassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 15);
    }

    public static bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}