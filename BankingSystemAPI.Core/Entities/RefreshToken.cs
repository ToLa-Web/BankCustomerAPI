namespace BankingSystemAPI.Core.Entities;

public class RefreshToken
{
    public int RefreshTokenId { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } =  string.Empty;
    public DateTime ExpiresAt  { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => RevokedAt == null && !IsExpired;
    
    public User? User { get; set; }
}