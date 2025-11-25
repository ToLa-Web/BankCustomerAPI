using System.Security.Cryptography;

namespace BankingSystemAPI.Services.Helpers;

public class TokenGenerator
{
    public static string GenerateToken(int length)
    {
        var randomBytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}