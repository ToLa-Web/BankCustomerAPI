using System.Security.Cryptography;
using System.Text;

namespace BankingSystemAPI.Services.Helpers;

public static class PasswordHasher
{
    // Hash a password with a generated salt
    public static (string hash, string salt) HashPassword(string password)
    {
        //1 Generate random salt
        byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
        string salt = Convert.ToBase64String(saltBytes);
        
        //2 Combine password with salt 
        var combine = Encoding.UTF8.GetBytes(password + salt);
        
        // 3. Hash with SHA256 (for production, consider BCrypt or Argon2)
        using var sha = SHA256.Create();
        var hasdBytes = sha.ComputeHash(combine);
        string hash = Convert.ToBase64String(hasdBytes);
        
        return (hash, salt);
    }
    
    //Verify password
    public static bool VerifyPassword(string password, string hash, string salt)
    {
        var combine = Encoding.UTF8.GetBytes(password + salt);
        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(combine);
        string computedHash = Convert.ToBase64String(hashBytes);
        return computedHash == hash;
    }
}